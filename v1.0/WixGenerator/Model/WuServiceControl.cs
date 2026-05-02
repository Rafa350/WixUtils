namespace WixGenerator.Model.Items {

    public sealed class WuServiceControl: WuEntity, IWuChildOf<WuComponentBase> {

        public enum ActionValue {
            Install,
            Uninstall,
            Both
        }

        public required string Name { get; set; }
        public ActionValue StartOn { get; set; } = ActionValue.Install;
        public ActionValue StopOn { get; set; } = ActionValue.Uninstall;
        public ActionValue RemoveOn { get; set; } = ActionValue.Uninstall;

        public override void AcceptVisitor(WuVisitor visitor) =>
            visitor.Visit(this);
    }
}
