using WixGenerator.Model.Items;

namespace WixGenerator.Model {

    public class WuVisitor {

        public WuVisitor() {
        }

        public virtual void Visit(WuProject project) {

            foreach (var entity in project.Entities)
                entity.AcceptVisitor(this);
        }

        public virtual void Visit(WuLaunch launch) {

        }

        public virtual void Visit(WuVariable variable) {

        }

        public virtual void Visit(WuProperty property) {

        }

        public virtual void Visit(WuSpecialFolder folder) {

            foreach (var entity in folder.Entities)
                entity.AcceptVisitor(this);
        }

        public virtual void Visit(WuFolder folder) {

            foreach (var entity in folder.Entities)
                entity.AcceptVisitor(this);
        }

        public virtual void Visit(WuFeature feature) {

            foreach (var f in feature.Features)
                f.AcceptVisitor(this);
        }

        public virtual void Visit(WuComponentGroup group) {

            foreach (var entity in group.Entities)
                entity.AcceptVisitor(this);
        }

        private void VisitComponent(WuComponentBase component) {

            foreach (var entity in component.Entities)
                entity.AcceptVisitor(this);
        }

        public virtual void Visit(WuComponent component) {

            VisitComponent(component);
        }

        public virtual void Visit(WuFileComponent component) {

            VisitComponent(component);
        }

        public virtual void Visit(WuAppFileComponent component) {

            VisitComponent(component);
        }

        public virtual void Visit(WuFileShortcut entity) {

        }

        public virtual void Visit(WuServiceInstall entity) {

        }

        public virtual void Visit(WuServiceControl entity) {

        }

        public virtual void Visit(WuRegisterAppPath entity) {

        }

        public virtual void Visit(WuRegisterKeyPath entity) {

        }

        public virtual void Visit(WuRemoveFolder entity) {

        }

        public virtual void Visit(WuDotNetCompatibilityCheck entity) {

        }

        public virtual void Visit(WuDirectory directory) {

        }
    }
}
