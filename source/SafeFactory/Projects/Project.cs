﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SafeFactory.Prediction;
using SafeFactory.SafetyRules;
using SafeFactory.VideoCapture;
using YoloDotNet;

namespace SafeFactory.Projects
{
    public class Project
    {
        public ObservableCollection<ViolationReport> Violations { get; } = [];

        public ProjectInfo Info { get; }

        public bool Processed { get; private set; }

        public Project(ProjectInfo info)
        {
            Info = info;
        }

        public async Task ProcessAsync(IProgress<(int, int)> progressTracker)
        {
            Violations.Clear();
            var box = new Yolo(new()
            {
                OnnxModel = "Models/box.onnx",
                ModelType = YoloDotNet.Enums.ModelType.ObjectDetection,
                Cuda = false,
            });
            var pose = new Yolo(new()
            {
                OnnxModel = "Models/pose.onnx",
                ModelType = YoloDotNet.Enums.ModelType.PoseEstimation,
                Cuda = false,
            });
            double fps = 0.5;
            using var processor = new FrameProcessor(box, pose);
            var split = FrameSplitter.Split(Info.VideoPath, 0.5);
            int i = 0;
            TimeSpan timeFromStart = default;
            FrameContext context = new();
            Rule[] safetyRules = [
                new CloseDoorRule(),
                new HelmetRule(),
                new StepOverRule()
            ];
            Rule.RuleOptions ruleOptions = new()
            {
                KillZone = ProjectManager.Instance.RoomConfig.DeadZone,
                MovementThreshold = 0.03f,
                RobotTagName = "robot",
                WorkerHeadTagName = "head",
                WorkerHelmetTagName = "helmet",
                WorkerTagName = "worker",
            };
            List<(ViolationReport, int)> activeViolations = [],
                newViolations = [];
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Info.ProjectPath), "Reports"));
            foreach (var frame in split)
            {
                i++;
                progressTracker.Report((i, split.Count));
                var frameInfo = await processor.ProcessFrameAsync(frame, timeFromStart, ProjectManager.Instance.RoomConfig);
                context.PushFrame(frameInfo);
                var results = from rule in safetyRules
                              let result = rule.CheckFrame(context, ruleOptions)
                              where result != System.ComponentModel.DataAnnotations.ValidationResult.Success
                              select result;
                newViolations.Clear();
                foreach (var result in results)
                {
                    ReportType type = Enum.Parse<ReportType>(result.MemberNames.First(), true);
                    int firstHelmetViolation = activeViolations.FindIndex(x => x.Item1.Type == type);
                    if (firstHelmetViolation == -1)
                    {
                        var violation = new ViolationReport(type, timeFromStart, timeFromStart);
                        newViolations.Add((violation, 1));
                    }
                    else
                    {
                        newViolations.Add((activeViolations[firstHelmetViolation].Item1, activeViolations[firstHelmetViolation].Item2 + 1));
                    }
                }

                foreach (var violation in activeViolations.Except(newViolations))
                {
                    Violations.Add(violation.Item1 with { EndTimestamp = timeFromStart, CapturedFrame = context.GetLastFrame(3)?.Frame ?? context.GetLastFrame(2)?.Frame ?? frame });
                }

                (activeViolations, newViolations) = (newViolations, activeViolations);
                timeFromStart = timeFromStart.Add(TimeSpan.FromSeconds(1 / fps));
            }

            string dir = Path.GetDirectoryName(Info.ProjectPath)!;
            foreach (var v in Violations)
            {
                string s = JsonConvert.SerializeObject(v);
                File.WriteAllText(Path.Combine(dir, "Reports", $"{v.BeginTimestamp}_Report_{v.Type}.json".Replace(':', '_')), s);
                using var stream = File.Create(Path.Combine(dir, "Reports", $"{v.BeginTimestamp}_Report_{v.Type}_Frame.png".Replace(':', '_')));
                v.CapturedFrame.Encode(stream, SkiaSharp.SKEncodedImageFormat.Png, 100);
            }

            Processed = true;
        }

        public async Task ReadReportsAsync(IProgress<(int, int)> progressTracker)
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(Info.ProjectPath, "Reports"), "*.json"))
            {
                string t = File.ReadAllText(file);
                var violation = JsonConvert.DeserializeObject<ViolationReport>(t);
                // TODO: add image clip.
                Violations.Add(violation);
            }

            Processed = true;
        }

        internal async Task RestoreAsync(IProgress<(int, int)> progressTracker)
        {
            if (Processed)
                return;
            if (Directory.Exists(Path.Combine(Info.ProjectPath, "Reports")))
            {
                await ReadReportsAsync(progressTracker);
            }
            else
            {
                await ProcessAsync(progressTracker);
            }
        }
    }
}
