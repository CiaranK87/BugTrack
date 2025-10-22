import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { Modal } from "semantic-ui-react";

export default observer(function ModalContainer() {
  const { modalStore } = useStore();

  return (
    <Modal
      open={modalStore.open}
      onClose={modalStore.closeModal}
      size="mini"
      dimmer="blurring"
      centered={true}
    >
      <Modal.Content style={{ padding: '1.5rem' }}>
        {modalStore.body}
      </Modal.Content>
    </Modal>
  );
});