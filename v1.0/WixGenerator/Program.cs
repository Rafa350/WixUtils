//using System.CommandLine;
using System.Text.RegularExpressions;
using WixGenerator.Generators;
using WixGenerator.Model;
using WixGenerator.Model.Extensions;
using WixGenerator.Model.Items;

namespace WixGenerator {

    internal class Program {

        static void Main(string[] args) {

            string productName = "Label Writer Engine Install Test";
            string productVersion = "0.0";
            string manufacturerName = "RSOpenWare";
            //string productCode = "{1F79B0D5-4820-49E6-9F88-C12CD3FB7479}";
            //string productUpgradeCode = "{3494AC38-ED10-44D5-A7D3-A2A2A3740965}";
            string productCode = "{8B46AA2E-1C16-4A71-9F0F-CB83D25AC7F5}";
            string productUpgradeCode = "{5E68F6AD-D6B3-4A7E-8402-732F544F38FC}";

            string binSource = @"C:\Users\Rafael\Documents\Projectes\Net\LwEngine\v5.1\Artifacts\publish\SetupWix";
            string configSource = @"C:\Users\Rafael\Documents\Projectes\Net\LwEngine\v5.1\DefaultCfg";

            // Crea el projecte
            //
            var project = new WuProject();
            project.Name = "Label Writer Studio";
            project.ProductCode = new Guid(productCode);
            project.UpgradeCode = new Guid(productUpgradeCode);
            project.IconFile = @"Icons\LwIcon32x32.ico";

            // Comprova la compatibilitat amb DOTNET
            //
            project.Add(new WuDotNetCompatibilityCheck {
                PropertyId = "DOTNETRUNTIMECHECK",
                Version = "10.0.0",
                RuntimeType = WuDotNetCompatibilityCheck.RuntimeTypeValue.Desktop,
                RollForward = WuDotNetCompatibilityCheck.RollForwardValue.Major,
                Platform = WuDotNetCompatibilityCheck.PlatformValue.x64
            });
            project.Add(new WuLaunch {
                ConditionExpr = "DOTNETRUNTIMECHECK = 0",
                Message = "This product requires .NET Desktop Runtime 10.0. Please install the NET Core, then run this installer again."
            });

            // Declara els directoris d'instal·lacio
            //
            var appDataDir = $"[CommonAppDataFolder]\\{manufacturerName}\\{productName}\\v{productVersion}";
            var installBinDir = $"[ProgramFiles64Folder]\\{manufacturerName}\\{productName}\\v{productVersion}\\bin";
            var installBinEsDir = $"[ProgramFiles64Folder]\\{manufacturerName}\\{productName}\\v{productVersion}\\bin\\es";
            var sharedDir = $"[CommonFiles64Folder]\\{manufacturerName}\\{productName}\\v{productVersion}";
            var shortcutsDir = $"[ProgramMenuFolder]\\{productName}";

            // Declara les propietats
            //
            project
                .Add(new WuProperty {
                    Name = "WIXUI_INSTALLDIR",
                    Value = "INSTALLDIR"
                });


            // Declara les variables
            //
            project
                .Add(new WuVariable {
                    Name = "WixUIBannerBmp",
                    Value = "Bitmaps\\topbanner.bmp"
                })
                .Add(new WuVariable {
                    Name = "WixUIDialogBmp",
                    Value = "Bitmaps\\leftbanner.bmp"
                })
                .Add(new WuVariable {
                    Name = "WixUILicenseRtf",
                    Value = "EULA\\eula.rtf"
                });

            // Declara els grups de components
            //
            project
                .AddComponentGroup("LwEngineTool", group => {
                    group
                        .AddExecutableFileComponent("LwEngineTool.exe", binSource, installBinDir)
                        .AddComponent(component => {
                            component
                                .Add(new WuFileShortcut {
                                    InstallDir = shortcutsDir,
                                    Target = $"{installBinDir}\\LwEngineTool.exe",
                                    Title = "LwEngineTool",
                                    Description = "LwEngine CLI management tool"
                                })
                                .Add(new WuRegisterKeyPath {
                                    Path = $"{installBinDir}\\LwEngineTool.exe"
                                });
                        })
                        .AddFileComponent("LwEngineTool.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineServices", group => {
                    group
                        .AddExecutableFileComponent("LwEngineServices.exe", binSource, installBinDir, component => {
                            component
                                .Add(new WuServiceInstall {
                                    Name = "LwEngineServices",
                                    Description = "LwEngine print services.",
                                    StartMode = WuServiceInstall.StartModeValue.Auto
                                })
                                .Add(new WuServiceControl {
                                    Name = "LwEngineServices"
                                });
                        })
                        .AddFileComponent("LwEngineServices.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineCore", group => {
                    group
                        .AddFileComponent("LwEngine.Barcode.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Batch.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Common.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Core.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Data.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Print.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Render.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Render.Skia.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.config.xml", configSource, sharedDir)
                        .AddFileComponent("LwEngine.windows.config.xml", configSource, sharedDir)
                        .AddFileComponent("LwEngine.printers.xml", configSource, sharedDir);
                })
                .AddComponentGroup("LwEngineDriver_CAB", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.CAB.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Driver.CAB.devices.xml", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineDriver_TEC", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.TEC.dll", binSource, installBinDir)
                        .AddFileComponent("LwEngine.Driver.TEC.devices.xml", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineDriver_PDF", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.PDF.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineDriver_SVG", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.SVG.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineDriver_Windows", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.Windows.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineDriver_Sato", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.Sato.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineDriver_Zebra", group => {
                    group
                        .AddFileComponent("LwEngine.Driver.Zebra.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineData_ODBC", group => {
                    group
                        .AddFileComponent("LwEngine.Data.ODBC.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineData_PostgreSQL", group => {
                    group
                        .AddFileComponent("LwEngine.Data.PostgreSQL.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineData_SQLite", group => {
                    group
                        .AddFileComponent("LwEngine.Data.SQLite.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwEngineData_CSV", group => {
                    group
                        .AddFileComponent("LwEngine.Data.CSV.dll", binSource, installBinDir);
                })
                .AddComponentGroup("LwFakeNetPrinter", group => {
                    group
                        .AddExecutableFileComponent("LwFakeNetPrinter.exe", binSource, installBinDir)
                        .AddComponent(component => {
                            component
                                .Add(new WuFileShortcut {
                                    InstallDir = shortcutsDir,
                                    Target = $"{installBinDir}\\LwFakeNetPrinter.exe",
                                    Title = "LwFakeNetPrinter",
                                    Description = "Net printer emulator."
                                })
                                .Add(new WuRegisterKeyPath {
                                    Path = $"{installBinDir}\\LwNetFakePrinter.exe"
                                });
                        })
                        .AddFileComponent("LwFakeNetPrinter.dll", binSource, installBinDir);
                })
                .AddComponentGroup("BaseComponents", group => {

                    var files = Directory.GetFiles(binSource, "*.dll", SearchOption.AllDirectories);
                    var exclusions = new Regex("Lw");

                    var fileNames = new List<string>();
                    foreach (var file in files) {
                        var name = Path.GetFileName(file);
                        if (!exclusions.IsMatch(name) && !fileNames.Contains(file)) {
                            fileNames.Add(file);
                            var sourceDir = Path.GetDirectoryName(file);
                            var installDir = Path.GetDirectoryName(Path.Combine(installBinDir, Path.GetRelativePath(binSource, file)));
                            group.AddFileComponent(name, sourceDir, installDir);
                        }
                    }
                    fileNames.Clear();
                })
                .AddComponentGroup("Cleanup", group => {
                    group.AddComponent(component => {
                        component.Add(new WuRemoveFolder {
                            Directory = shortcutsDir,
                            PerformOn = WuRemoveFolder.PerformOnValue.Uninstall
                        });
                    });
                });

            // Declara les caracteristiques de la instal·lacio
            //
            project
                .AddFeature("Base", "", feature => {
                    feature.Visibility = WuFeature.VisibilityValue.Hidden;
                    feature.Add(project.FindGroup("BaseComponents"));
                    feature.Add(project.FindGroup("LwEngineCore"));
                    feature.Add(project.FindGroup("Cleanup"));
                })
                .AddFeature("Tools", "Print and configuration CLI tool.", feature => {
                    feature.Add(project.FindGroup("LwEngineTool"));
                })
                .AddFeature("Services", "Print services.", feature => {
                    feature.Add(project.FindGroup("LwEngineServices"));
                })
                .AddFeature("Data access", "Data access drivers.", feature => {
                    feature
                        .AddFeature("ODBC", "ODBC data access driver", feature => {
                            feature.Add(project.FindGroup("LwEngineData_ODBC"));
                        })
                        .AddFeature("PostgreSQL", "PostgreSQL data access driver", feature => {
                            feature.Add(project.FindGroup("LwEngineData_PostgreSQL"));
                        })
                        .AddFeature("SQLite", "SQLite data access driver", feature => {
                            feature.Add(project.FindGroup("LwEngineData_SQLite"));
                        })
                        .AddFeature("CSV", "CSV data access driver", feature => {
                            feature.Add(project.FindGroup("LwEngineData_CSV"));
                        });
                })
                .AddFeature("Printer drivers", "Printer machine drivers.", feature => {
                    feature
                        .AddFeature("Windows", "Windows printer driver", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_Windows"));
                        })
                        .AddFeature("CAB", "CAB printer driver", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_CAB"));
                        })
                        .AddFeature("Toshiba-TEC", "Toshiba-TEC printer river", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_TEC"));
                        })
                        .AddFeature("Sato", "Sato printer driver", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_Sato"));
                        })
                        .AddFeature("Zebra", "Zebra printer driver", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_Zebra"));
                        })
                        .AddFeature("PDF", "PDF printer driver", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_PDF"));
                        })
                        .AddFeature("SVG", "SVG printer driver", feature => {
                            feature.Add(project.FindGroup("LwEngineDriver_SVG"));
                        });
                })
                .AddFeature("Test and debug tools", "Test and debug tools.", feature => {
                    feature
                        .Add(project.FindGroup("LwFakeNetPrinter"));
                });

            var generator = new Generators.WixGenerator();
            generator.Generate(project, @"c:\Users\Rafael\Documents\Projectes\Net\WixUtils\v1.0\SetupProjectDemo\Product.wxs");

            /*var rootArgument = new Argument<string>("root");
            rootArgument.Description = "Root path.";
            rootArgument.Arity = ArgumentArity.ExactlyOne;

            var outputArgument = new Argument<string>("output");
            outputArgument.Description = "Output file.";
            outputArgument.Arity = ArgumentArity.ExactlyOne;

            var depOption = new Option<List<string>>("--dep", "-d");
            depOption.Description = "Add dependency file.";
            depOption.Arity = ArgumentArity.ZeroOrMore;
            depOption.Validators.Add(result => {
                var files = result.GetValueOrDefault<List<string>>();
                if (files != null) {
                    foreach (var file in files) {
                        if (!File.Exists(file)) {
                            result.AddError($"File '{file}' does not exist.");
                            return;
                        }
                    }
                }
            });

            var folderOption = new Option<List<string>>("--folder", "-f");
            folderOption.Description = "Add folder content.";
            folderOption.Arity = ArgumentArity.ZeroOrMore;
            folderOption.Validators.Add(result => {
                var folders = result.GetValueOrDefault<List<string>>();
                if (folders != null) {
                    foreach (var folder in folders) {
                        if (!Directory.Exists(folder)) {
                            result.AddError( $"Folder '{folder}' does not exist.");
                            return;
                        }
                    }
                }
            });

            var exclusionOption = new Option<List<string>>("--exclude", "-e");
            exclusionOption.Description = "Add exclusion pattern.";
            exclusionOption.Arity = ArgumentArity.ZeroOrMore;

            var groupIdOption = new Option<string>("--group-id");
            groupIdOption.Description = "Component group ID.";
            groupIdOption.DefaultValueFactory = r => "groupId";

            var dirIdOption = new Option<string>("--directory-id");
            dirIdOption.Description = "Directory ID.";
            dirIdOption.DefaultValueFactory = r => "INSTALLFOLDER";

            var rootCommand = new RootCommand();
            rootCommand.Description = "Create Wix component file";
            rootCommand.Arguments.Add(rootArgument);
            rootCommand.Arguments.Add(outputArgument);
            rootCommand.Options.Add(depOption);
            rootCommand.Options.Add(folderOption);
            rootCommand.Options.Add(exclusionOption);
            rootCommand.Options.Add(groupIdOption);
            rootCommand.Options.Add(dirIdOption);
            rootCommand.SetAction(async (parseResult, cancellationToken) => {

                var root = parseResult.GetValue(rootArgument);
                var output = parseResult.GetValue(outputArgument);
                var deps = parseResult.GetValue(depOption);
                var folders = parseResult.GetValue(folderOption);
                var exclusions = parseResult.GetValue(exclusionOption);
                var groupId = parseResult.GetValue(groupIdOption);
                var dirId = parseResult.GetValue(dirIdOption);

                await DoRootCommand(root, output, deps, folders, exclusions, dirId, groupId);
            });

            var parseResult = rootCommand.Parse(args);
            parseResult.Invoke();*/
        }
        /*
        private static async Task DoRootCommand(string root, string output, IEnumerable<string> depFiles, 
            IEnumerable<string> folders, IEnumerable<string> exclusions, string dirId, string groupId) { 

            var builder = new DependencyListBuilder(root);

            foreach (var depFile in depFiles) 
                builder.AddDependencyFile(depFile);

            foreach (var folder in folders) 
                builder.AddFolder(folder);

            foreach (var exclusion in exclusions)   
                builder.AddExclusionPattern(exclusion);

            var list = builder.Build();
            WixSourceGenerator.Generate(list, output, groupId, "bindir", dirId);
        }
        */
    }
}