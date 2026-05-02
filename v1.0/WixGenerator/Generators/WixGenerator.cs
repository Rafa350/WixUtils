using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using WixGenerator.Model;
using WixGenerator.Model.Items;

namespace WixGenerator.Generators {

    public sealed class WixGenerator {

        public void Generate(WuProject project, string outputPath) {

            ArgumentNullException.ThrowIfNull(project, nameof(project));
            ArgumentNullException.ThrowIfNullOrEmpty(outputPath, nameof(outputPath));

            var settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    "
            };
            using (var writer = XmlWriter.Create(outputPath, settings)) {

                GenerateStartPackage(writer, project);
                GenerateVariables(writer, project);
                GenerateProperties(writer, project);
                GenerateDirectories(writer, project);
                GenerateFeatures(writer, project);
                GenerateComponents(writer, project);
                GenerateEndPackage(writer, project);
            }
        }

        private void GenerateStartPackage(XmlWriter writer, WuProject project) {

            writer.WriteStartDocument();

            writer.WriteStartElement("Wix", "http://wixtoolset.org/schemas/v4/wxs");
            writer.WriteAttributeString("xmlns", "ui", null, "http://wixtoolset.org/schemas/v4/wxs/ui");
            writer.WriteAttributeString("xmlns", "netfx", null, "http://wixtoolset.org/schemas/v4/wxs/netfx");

            writer.WriteStartElement("Package");
            writer.WriteAttributeString("Name", project.Name);
            writer.WriteAttributeString("Language", CultureInfo.GetCultureInfo("en-US").LCID.ToString());
            writer.WriteAttributeString("Version", "5.0");
            writer.WriteAttributeString("ProductCode", project.ProductCode.ToString());
            writer.WriteAttributeString("UpgradeCode", project.UpgradeCode.ToString());
            writer.WriteAttributeString("InstallerVersion", "500");
            writer.WriteAttributeString("Manufacturer", "Demo");

            writer.WriteStartElement("Media");
            writer.WriteAttributeString("Id", "1");
            writer.WriteAttributeString("EmbedCab", "yes");
            writer.WriteAttributeString("Cabinet", "Setup.cab");
            writer.WriteEndElement();

            if (!String.IsNullOrEmpty(project.IconFile)) {
                writer.WriteStartElement("Icon");
                writer.WriteAttributeString("Id", "InstallerIcon");
                writer.WriteAttributeString("SourceFile", project.IconFile);
                writer.WriteEndElement();
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Id", "ARPPRODUCTICON");
                writer.WriteAttributeString("Value", "InstallerIcon");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("MajorUpgrade");
            writer.WriteAttributeString("DowngradeErrorMessage", "A newer version of [ProductName] is already installed.");
            writer.WriteEndElement();

            foreach (var entity in project.Entities) {
                var visitor = new ProjectChildsVisitor(writer);
                entity.AcceptVisitor(visitor);
            }

            writer.WriteStartElement("ui", "WixUI", null);
            writer.WriteAttributeString("Id", "WixUI_Mondo");
            writer.WriteEndElement();
            writer.WriteStartElement("UIRef");
            writer.WriteAttributeString("Id", "WixUI_ErrorProgressText");
            writer.WriteEndElement();
        }

        private void GenerateEndPackage(XmlWriter writer, WuProject project) {

            writer.WriteEndElement();     // Tanca <atckage>
            writer.WriteEndElement();     // Tanca <Wix>

            writer.WriteEndDocument();
        }

        private void GenerateVariables(XmlWriter writer, WuProject project) {

            foreach (var variable in project.Entities.OfType<WuVariable>()) {
                writer.WriteStartElement("WixVariable");
                writer.WriteAttributeString("Id", variable.Name);
                writer.WriteAttributeString("Value", variable.Value);
                writer.WriteEndElement();
            }
        }

        private void GenerateProperties(XmlWriter writer, WuProject project) {

            foreach (var property in project.Entities.OfType<WuProperty>()) {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Id", property.Name);
                writer.WriteAttributeString("Value", property.Value);
                writer.WriteEndElement();
            }
        }

        private void GenerateFeatures(XmlWriter writer, WuProject project) {

            var visitor = new GenerateFeaturesVisitor(writer);
            visitor.Visit(project);
        }

        private void GenerateDirectories(XmlWriter writer, WuProject project) {

            var visitor = new GenerateDirectoriesVisitor(writer);
            visitor.Visit(project);
        }

        private void GenerateComponents(XmlWriter writer, WuProject project) {

            var visitor = new GenerateComponentsVisitor(writer);
            visitor.Visit(project);
        }

        private sealed class ProjectChildsVisitor: WuVisitor {

            private readonly XmlWriter _writer;

            public ProjectChildsVisitor(XmlWriter writer) {

                _writer = writer;
            }

            public override void Visit(WuDotNetCompatibilityCheck child) {

                var rollForward = child.RollForward switch {
                    WuDotNetCompatibilityCheck.RollForwardValue.Major => "major",
                    WuDotNetCompatibilityCheck.RollForwardValue.LatestMajor => "latestMajor",
                    WuDotNetCompatibilityCheck.RollForwardValue.Minor => "minor",
                    WuDotNetCompatibilityCheck.RollForwardValue.LatestMinor => "latestMinor",
                    WuDotNetCompatibilityCheck.RollForwardValue.LatestPatch => "latestPath",
                    _ => "disable"
                };

                var runtimeType = child.RuntimeType switch {
                    WuDotNetCompatibilityCheck.RuntimeTypeValue.Desktop => "desktop",
                    WuDotNetCompatibilityCheck.RuntimeTypeValue.ASPNet => "aspnet",
                    _ => "core"
                };

                var platform = child.Platform switch {
                    WuDotNetCompatibilityCheck.PlatformValue.x86 => "x86",
                    WuDotNetCompatibilityCheck.PlatformValue.amd64 => "x64",
                    _ => "x64"
                };

                _writer.WriteStartElement("netfx", "DotNetCompatibilityCheck", null);
                _writer.WriteAttributeString("Property", child.PropertyId);
                _writer.WriteAttributeString("RollForward", rollForward);
                _writer.WriteAttributeString("RuntimeType", runtimeType);
                _writer.WriteAttributeString("Platform", platform);
                _writer.WriteAttributeString("Version", child.Version);
                _writer.WriteEndElement();
            }

            public override void Visit(WuLaunch child) {

                _writer.WriteStartElement("Launch");
                _writer.WriteAttributeString("Condition", child.ConditionExpr);
                _writer.WriteAttributeString("Message", child.Message);
                _writer.WriteEndElement();
            }
        }

        private sealed class GenerateComponentsVisitor: WuVisitor {

            private readonly XmlWriter _writer;

            public GenerateComponentsVisitor(XmlWriter writer) {

                _writer = writer;
            }

            public override void Visit(WuComponentGroup group) {

                var groupId = String.Format("group_{0:X8}", group.GetHashCode());

                _writer.WriteStartElement("ComponentGroup");
                _writer.WriteAttributeString("Id", groupId);

                base.Visit(group);

                _writer.WriteEndElement();
            }

            public override void Visit(WuComponent component) {

                var componentId = String.Format("component_{0:X8}", component.GetHashCode());

                _writer.WriteStartElement("Component");
                _writer.WriteAttributeString("Id", componentId);
                _writer.WriteAttributeString("Guid", component.Guid.ToString());

                base.Visit(component);

                _writer.WriteEndElement();
            }

            public override void Visit(WuFileComponent component) {

                var componentId = String.Format("component_{0:X8}", component.GetHashCode());
                var fileId = String.Format("file_{0:X8}", component.GetHashCode());

                var fullTargetDir = component.InstallDir;
                var lastTargetDir = fullTargetDir.Substring(fullTargetDir.LastIndexOf('\\') + 1);
                var directoryId = $"directory_{Math.Abs(HashCode.Combine(fullTargetDir, lastTargetDir))}";

                _writer.WriteStartElement("Component");
                _writer.WriteAttributeString("Id", componentId);
                _writer.WriteAttributeString("Guid", component.Guid.ToString());
                _writer.WriteAttributeString("Directory", directoryId);

                _writer.WriteStartElement("File");
                _writer.WriteAttributeString("Id", fileId);
                _writer.WriteAttributeString("Source", Path.Combine(component.SourceDir, component.Name));
                _writer.WriteAttributeString("KeyPath", "yes");
                _writer.WriteEndElement();

                base.Visit(component);

                _writer.WriteEndElement();
            }

            public override void Visit(WuExecutableFileComponent component) {

                var componentId = String.Format("component_{0:X8}", component.GetHashCode());
                var fileId = String.Format("file_{0:X8}", component.GetHashCode());

                var fullTargetDir = component.InstallDir;
                var lastTargetDir = fullTargetDir.Substring(fullTargetDir.LastIndexOf('\\') + 1);
                var directoryId = $"directory_{Math.Abs(HashCode.Combine(fullTargetDir, lastTargetDir))}";

                _writer.WriteStartElement("Component");
                _writer.WriteAttributeString("Id", componentId);
                _writer.WriteAttributeString("Guid", component.Guid.ToString());
                _writer.WriteAttributeString("Directory", directoryId);

                _writer.WriteStartElement("File");
                _writer.WriteAttributeString("Id", fileId);
                _writer.WriteAttributeString("Source", Path.Combine(component.SourceDir, component.Name));
                _writer.WriteAttributeString("KeyPath", "yes");
                _writer.WriteEndElement();

                base.Visit(component);

                _writer.WriteEndElement();
            }

            public override void Visit(WuFileShortcut shortcut) {

                var shortcutId = String.Format("shortcut_{0:X8}", shortcut.GetHashCode());

                var fulltDir = shortcut.InstallDir;
                var lastDir = fulltDir.Substring(fulltDir.LastIndexOf('\\') + 1);
                var directoryId = $"directory_{Math.Abs(HashCode.Combine(fulltDir, lastDir))}";

                _writer.WriteStartElement("Shortcut");
                _writer.WriteAttributeString("Id", shortcutId);
                _writer.WriteAttributeString("Directory", directoryId);
                _writer.WriteAttributeString("Name", shortcut.Title);
                _writer.WriteAttributeString("Description", shortcut.Description);
                _writer.WriteAttributeString("Target", shortcut.Target);
                _writer.WriteEndElement();
            }

            public override void Visit(WuRegisterKeyPath entity) {

                _writer.WriteStartElement("RegistryValue");
                _writer.WriteAttributeString("Root", "HKCU");
                _writer.WriteAttributeString("Key", entity.Path);
                _writer.WriteAttributeString("Name", "shortcut");
                _writer.WriteAttributeString("Type", "integer");
                _writer.WriteAttributeString("Value", "1");
                _writer.WriteAttributeString("KeyPath", "yes");
                _writer.WriteEndElement();
            }

            public override void Visit(WuRemoveFolder entity) {

                var fulltDir = entity.Directory;
                var lastDir = fulltDir.Substring(fulltDir.LastIndexOf('\\') + 1);
                var directoryId = $"directory_{Math.Abs(HashCode.Combine(fulltDir, lastDir))}";

                var performOn = entity.PerformOn switch {
                    WuRemoveFolder.PerformOnValue.Install => "install",
                    WuRemoveFolder.PerformOnValue.Uninstall => "uninstall",
                    _ => "both"
                };

                _writer.WriteStartElement("RemoveFolder");
                _writer.WriteAttributeString("Directory", directoryId);
                _writer.WriteAttributeString("On", performOn);
                _writer.WriteEndElement();
            }

            public override void Visit(WuServiceInstall serviceInstall) {

                var startMode = serviceInstall.StartMode switch {
                    WuServiceInstall.StartModeValue.Disabled => "disabled",
                    WuServiceInstall.StartModeValue.Demand => "demand",
                    _ => "auto",
                };

                _writer.WriteStartElement("ServiceInstall");
                _writer.WriteAttributeString("Name", serviceInstall.Name);
                _writer.WriteAttributeString("Type", "ownProcess");
                _writer.WriteAttributeString("DisplayName", serviceInstall.Name);
                if (!String.IsNullOrEmpty(serviceInstall.Description))
                    _writer.WriteAttributeString("Description", serviceInstall.Description);
                _writer.WriteAttributeString("Start", startMode);
                _writer.WriteAttributeString("ErrorControl", "normal");
                _writer.WriteEndElement();
            }

            public override void Visit(WuServiceControl serviceControl) {

                var start = serviceControl.StartOn switch {
                    WuServiceControl.ActionValue.Install => "install",
                    WuServiceControl.ActionValue.Uninstall => "uninstall",
                    _ => "both",
                };

                var stop = serviceControl.StopOn switch {
                    WuServiceControl.ActionValue.Install => "install",
                    WuServiceControl.ActionValue.Uninstall => "uninstall",
                    _ => "both",
                };

                var remove = serviceControl.RemoveOn switch {
                    WuServiceControl.ActionValue.Install => "install",
                    WuServiceControl.ActionValue.Uninstall => "uninstall",
                    _ => "both",
                };

                _writer.WriteStartElement("ServiceControl");
                _writer.WriteAttributeString("Name", serviceControl.Name);
                _writer.WriteAttributeString("Start", start);
                _writer.WriteAttributeString("Stop", stop);
                _writer.WriteAttributeString("Remove", remove);
                _writer.WriteAttributeString("Wait", "yes");
                _writer.WriteEndElement();
            }

            public override void Visit(WuRegisterAppPath registerAppPath) {

                _writer.WriteStartElement("RegistryKey");
                _writer.WriteAttributeString("Root", "HKLM");
                _writer.WriteAttributeString("Key", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths");
                _writer.WriteStartElement("RegistryKey");
                _writer.WriteAttributeString("Key", "x");
                _writer.WriteAttributeString("ForceCreateOnInstall", "yes");
                _writer.WriteAttributeString("ForceDeleteOnUninstall", "yes");
                _writer.WriteStartElement("RegistryValue");
                _writer.WriteAttributeString("Type", "string");
                _writer.WriteAttributeString("Value", "x");
                _writer.WriteEndElement();
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
        }

        private sealed class GenerateFeaturesVisitor: WuVisitor {

            private readonly XmlWriter _writer;

            public GenerateFeaturesVisitor(XmlWriter writer) {

                _writer = writer;
            }

            public override void Visit(WuFeature feature) {

                var featureId = String.Format("feature_{0:X8}", feature.GetHashCode());

                var visibility = feature.Visibility switch {
                    WuFeature.VisibilityValue.Expanded => "expand",
                    WuFeature.VisibilityValue.Collapsed => "collapse",
                    _ => "hidden"
                };

                _writer.WriteStartElement("Feature");
                _writer.WriteAttributeString("Id", featureId);
                _writer.WriteAttributeString("Title", feature.Title);
                _writer.WriteAttributeString("Description", feature.Description);
                _writer.WriteAttributeString("Display", visibility);
                if (feature.Visibility == WuFeature.VisibilityValue.Hidden)
                    _writer.WriteAttributeString("AllowAbsent", "no");

                foreach (var g in feature.Groups) {

                    var groupId = String.Format("group_{0:X8}", g.GetHashCode());

                    _writer.WriteStartElement("ComponentGroupRef");
                    _writer.WriteAttributeString("Id", groupId);
                    _writer.WriteEndElement();
                }

                base.Visit(feature);

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Explora tots els element a la recerca de directoris. Despres genera les entrades
        /// corresponents a cada directory i subdirectori.
        /// </summary>
        /// 
        private sealed class GenerateDirectoriesVisitor: WuVisitor {

            private readonly XmlWriter _writer;
            private readonly List<string> _directories = new List<string>();
            private readonly DirectoryTreeBuilder _directoryTreeBuilder = new DirectoryTreeBuilder();

            public GenerateDirectoriesVisitor(XmlWriter writer) {

                _writer = writer;
            }

            public override void Visit(WuProject project) {

                void ProcessDirectoryNode(DirectoryNode node, string path) {

                    var name = node.Name;

                    if (name.StartsWith('[') && name.EndsWith(']')) {
                        path = name;
                        _writer.WriteStartElement("StandardDirectory");
                        _writer.WriteAttributeString("Id", name.Substring(1, name.Length - 2));
                    }
                    else {
                        path = String.Concat(path, "\\", name);
                        _writer.WriteStartElement("Directory");
                        _writer.WriteAttributeString("Id", String.Format("directory_{0:X8}", path.GetHashCode()));
                        _writer.WriteAttributeString("Name", name);
                    }

                    foreach (var subdirectory in node.Subdirectories)
                        ProcessDirectoryNode(subdirectory, path);

                    _writer.WriteEndElement();
                }

                // Construeix la llista de directoris del projecte
                //
                foreach (var group in project.Entities)
                    group.AcceptVisitor(this);

                // Genera les entrades  corresponents
                //
                foreach (var subdirectory in _directoryTreeBuilder.Root.Subdirectories)
                    ProcessDirectoryNode(subdirectory, String.Empty);
            }

            public override void Visit(WuDirectory directory) {

                AddDirectory(directory.Name);
                _directoryTreeBuilder.Append(directory.Name);
            }

            public override void Visit(WuFileComponent component) {

                AddDirectory(component.InstallDir);
                _directoryTreeBuilder.Append(component.InstallDir);

                base.Visit(component);
            }

            public override void Visit(WuExecutableFileComponent component) {

                AddDirectory(component.InstallDir);
                _directoryTreeBuilder.Append(component.InstallDir);

                base.Visit(component);
            }

            public override void Visit(WuFileShortcut shortcut) {

                AddDirectory(shortcut.InstallDir);
                _directoryTreeBuilder.Append(shortcut.InstallDir);
            }

            private void AddDirectory(string directory) {

                if (!_directories.Contains(directory))
                    _directories.Add(directory);
            }
        }
    }
}
