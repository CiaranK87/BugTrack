import { createContext, useContext } from "react";
import ProjectStore from "./projectStore";
import CommonStore from "./commonStore";

interface Store {
  projectStore: ProjectStore;
  commonStore: CommonStore;
}

export const store: Store = {
  projectStore: new ProjectStore(),
  commonStore: new CommonStore(),
};

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}
