import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { Modal } from "semantic-ui-react";

export default observer(function ModalContainer() {
  const { modalStore } = useStore();

  return (
    <Modal 
      open={modalStore.open} 
      onClose={modalStore.closeModal} 
      size="small"
      dimmer="blurring"
    >
      <Modal.Content>
        {modalStore.body}
      </Modal.Content>
    </Modal>
  );
});