namespace WixGenerator.Generators.Wix {

    public sealed class GuidFactory {

        private static GuidFactory? _instance;

        public static GuidFactory Instance {
            get {
                if (_instance == null) 
                    _instance = new GuidFactory();
                return _instance;
            }
        }
    }
}
