import { makeAutoObservable } from "mobx";

export default class ModalStore {
  open = false;
  body: JSX.Element | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  openModal = (content: JSX.Element) => {
    this.body = content;
    this.open = true;
  };

  closeModal = () => {
    this.body = null;
    this.open = false;
  };

  setBody = (content: JSX.Element) => {
    this.body = content;
  };
}

