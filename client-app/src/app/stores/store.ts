import { createContext, useContext } from "react";
import ProjectStore from "./projectStore";
import CommonStore from "./commonStore";
import UserStore from "./userStore";
import ModalStore from "./modalStore";
import TicketStore from "./ticketStore";
import CommentStore from "./commentStore";

interface Store {
  projectStore: ProjectStore;
  commonStore: CommonStore;
  userStore: UserStore;
  modalStore: ModalStore;
  ticketStore: TicketStore;
  commentStore: CommentStore;
}

export const store: Store = {
  projectStore: new ProjectStore(),
  commonStore: new CommonStore(),
  userStore: new UserStore(),
  modalStore: new ModalStore(),
  ticketStore: new TicketStore(),
  commentStore: new CommentStore(),
};

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}
