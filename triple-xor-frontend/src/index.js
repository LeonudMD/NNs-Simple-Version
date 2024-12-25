import React from "react";
import ReactDOM from "react-dom/client";
import "bootstrap/dist/css/bootstrap.min.css"; // Подключаем bootstrap-стили
import "./index.css"; // Ваши стили (опционально)
import App from "./App"; // Сам компонент приложения

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
    <React.StrictMode>
        <App />
    </React.StrictMode>
);
