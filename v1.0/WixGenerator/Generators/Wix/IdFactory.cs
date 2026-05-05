using WixGenerator.Model;

namespace WixGenerator.Generators.Wix {
    
    public sealed class IdFactory {

        private static IdFactory? _instance;

        private IdFactory() {

        }

        public string GetId(WuFeature feature) =>
            String.Format("ID_{0:X8}", HashCode.Combine("feature", feature));

        public string GetId(WuComponentBase component) =>
            String.Format("ID_{0:X8}", HashCode.Combine("component", component));

        public string GetId(WuComponentGroup group) =>
            String.Format("ID_{0:X8}", HashCode.Combine("group", group));

        public static IdFactory Instance {
            get {
                if (_instance == null) 
                    _instance = new IdFactory();
                return _instance;
            }
        }
    }
}
