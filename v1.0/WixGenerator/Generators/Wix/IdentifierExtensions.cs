using WixGenerator.Model;

namespace WixGenerator.Generators.Wix {

    internal static class IdentifierExtensions {

        public static string GetId(this WuFeature feature) =>
            IdFactory.Instance.GetId(feature);

        public static string GetId(this WuComponentGroup group) =>
            IdFactory.Instance.GetId(group);

        public static string GetId(this WuComponentBase component) =>
            IdFactory.Instance.GetId(component);

        public static string GetId(this WuFileShortcut shortcut) =>
            String.Format("ID_{0:X8}", HashCode.Combine("shortcut", shortcut));
    }
}
