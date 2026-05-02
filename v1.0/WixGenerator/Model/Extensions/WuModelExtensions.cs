namespace WixGenerator.Model.Extensions {

    public static class WuModelExtensions {

        /// <summary>
        /// Extensions per Project
        /// </summary>
        /// <param name="project"></param>
        /// 
        extension(WuProject project) {

            public WuProject AddFeature(string title, string description, Action<WuFeature>? action) {

                var entity = new WuFeature(title, description);
                action?.Invoke(entity);
                project.Add(entity);

                return project;
            }

            public WuProject AddComponentGroup(string name, Action<WuComponentGroup>? action = null) {

                var entity = new WuComponentGroup(name);
                action?.Invoke(entity);
                project.Add(entity);

                return project;
            }
        }

        /// <summary>
        /// Extensions per Feature
        /// </summary>
        /// <param name="feature"></param>
        /// 
        extension(WuFeature feature) {

            public WuFeature AddFeature(string title, string description, Action<WuFeature>? action = null) {

                var entity = new WuFeature(title, description);
                action?.Invoke(entity);
                feature.Add(entity);

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

                var entity = new WuComponent();
                group.Add(entity);
                action(entity);

                return group;
            }

            public WuComponentGroup AddFileComponent(string name, string sourceDir, string installDir, Action<WuFileComponent>? action = null) {

                var entity = new WuFileComponent(name, sourceDir, installDir);
                action?.Invoke(entity);
                group.Add(entity);

                return group;
            }

            public WuComponentGroup AddExecutableFileComponent(string name, string sourceDir, string installDir, Action<WuExecutableFileComponent>? action = null) {

                var entity = new WuExecutableFileComponent(name, sourceDir, installDir);
                action?.Invoke(entity);
                group.Add(entity);

                return group;
            }
        }
    }
}
