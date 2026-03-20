import { createContext, useContext } from "react";
import ProjectStore from "./projectStore";
import CommonStore from "./commonStore";
import UserStore from "./userStore";
import ModalStore from "./modalStore";
import TicketStore from "./ticketStore";
import CommentStore from "./commentStore";
import NotificationStore from "./notificationStore";

interface Store {
  projectStore: ProjectStore;
  commonStore: CommonStore;
  userStore: UserStore;
  modalStore: ModalStore;
  ticketStore: TicketStore;
  commentStore: CommentStore;
  notificationStore: NotificationStore;
}

export const store: Store = {
  projectStore: new ProjectStore(),
  commonStore: new CommonStore(),
  userStore: new UserStore(),
  modalStore: new ModalStore(),
  ticketStore: new TicketStore(),
  commentStore: new CommentStore(),
  notificationStore: new NotificationStore(),
};

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}
