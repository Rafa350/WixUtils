namespace WixGenerator.Model.Extensions {

    public static class WuModelExtensions {

        /// <summary>
        /// Extensions per Project
        /// </summary>
        /// <param name="project"></param>
        /// 
        extension(WuProject project) {

            public WuProject AddFeature(string title, string description) {

                var f = new WuFeature(title, description);
                project.Add(f);

                return project;
            }

            public WuProject AddFeature(string title, string description, Action<WuFeature> action) {

                var f = new WuFeature(title, description);
                action(f);
                project.Add(f);

                return project;
            }

            public WuProject AddComponentGroup(string name) {

                var g = new WuComponentGroup(name);
                project.Add(g);

                return project;
            }

            public WuProject AddComponentGroup(string name, Action<WuComponentGroup> action) {

                var g = new WuComponentGroup(name);
                action(g);
                project.Add(g);

                return project;
            }
        }

        /// <summary>
        /// Extensions per Feature
        /// </summary>
        /// <param name="feature"></param>
        /// 
        extension(WuFeature feature) {

            public WuFeature AddFeature(string title, string description) {

                var f = new WuFeature(title, description);
                feature.Add(f);

                return feature;
            }

            public WuFeature AddFeature(string title, string description, Action<WuFeature> action) {

                var f = new WuFeature(title, description);
                action(f);
                feature.Add(f);

                return feature;
            }
        }

        /// <summary>
        /// Extensions per  ComponentGroup
        /// </summary>
        /// <param name="group"></param>
        /// 
        extension(WuComponentGroup group) {

            public WuComponentGroup AddComponent(Action<WuComponent> action) {

                var c = new WuComponent();
                group.Add(c);
                action(c);

                return group;

            }

            public WuComponentGroup AddFileComponent(string name, string sourceDir, string targetDir) {

                var c = new WuFileComponent(name, sourceDir, targetDir);
                group.Add(c);

                return group;
            }

            public WuComponentGroup AddFilesComponent(IEnumerable<String> names, string sourceDir, string targetDir) {

                foreach (var name in names)
                    group.AddFileComponent(name, sourceDir, targetDir);

                return group;
            }

            public WuComponentGroup AddExecutableFileComponent(string name, string sourceDir, string targetDir) {

                var c = new WuExecutableFileComponent(name, sourceDir, targetDir);
                group.Add(c);

                return group;
            }

            public WuComponentGroup AddExecutableFileComponent(string name, string sourceDir, string targetDir, Action<WuExecutableFileComponent> action) {

                var c = new WuExecutableFileComponent(name, sourceDir, targetDir);
                action(c);
                group.Add(c);

                return group;
            }
        }
    }
}
