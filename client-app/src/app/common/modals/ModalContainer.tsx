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
    >
      <Modal.Content style={{ padding: '2rem' }}>
        {modalStore.body}
      </Modal.Content>
    </Modal>
  );
});