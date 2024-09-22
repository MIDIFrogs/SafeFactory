using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Compunet.YoloV8;
using Newtonsoft.Json;
using SafeFactory.SafetyRules;
using SafeFactory.VideoCapture;

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
            var box = new YoloPredictor("Models/box.onnx");
            var pose = new YoloPredictor("Models/pose.onnx");
            var yoloConfig = new YoloConfiguration()
            {
                Confidence = 0.25f,
                SkipImageAutoOrient = true,
            };
            double fps = 0.5;
            using var processor = new FrameProcessor(box, pose, yoloConfig);
            var split = FrameSpliter.Split(Info.VideoPath, 0.5);
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
            Directory.CreateDirectory(Path.Combine(Info.ProjectPath, "Reports"));
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
                        newViolations.Add((activeViolations[i].Item1, activeViolations[i].Item2 + 1));
                    }
                }

                foreach (var violation in activeViolations.Except(newViolations))
                {
                    Violations.Add(violation.Item1 with { EndTimestamp = timeFromStart, CapturedFrame = context.GetLastFrame(3)?.Frame ?? context.GetLastFrame(2)?.Frame ?? frame });
                }

                (activeViolations, newViolations) = (newViolations, activeViolations);
                timeFromStart.Add(TimeSpan.FromSeconds(1 / fps));
            }

            foreach (var v in Violations)
            {
                string s = JsonConvert.SerializeObject(v);
                File.WriteAllText(Path.Combine(Info.ProjectPath, "Reports", $"{v.BeginTimestamp}_Report_{v.Type}.json"), s);
                using var stream = File.Create(Path.Combine(Info.ProjectPath, "Reports", $"{v.BeginTimestamp}_Report_{v.Type}_Frame.png"));
                await v.CapturedFrame.SaveAsync(stream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
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
