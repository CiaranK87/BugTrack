import React from "react";
import ReactDOM from "react-dom/client";
import "semantic-ui-css/semantic.min.css";
import "react-calendar/dist/Calendar.css";
import "react-toastify/dist/ReactToastify.min.css";
import "react-datepicker/dist/react-datepicker.css";
import "./app/layout/styles.css";
import { StoreContext, store } from "./app/stores/store";
import { ThemeProvider } from "./app/context/ThemeContext";
import { RouterProvider } from "react-router-dom";
import { router } from "./app/router/Routes";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <StoreContext.Provider value={store}>
      <ThemeProvider>
        <RouterProvider router={router} />
      </ThemeProvider>
    </StoreContext.Provider>
  </React.StrictMode>
);
