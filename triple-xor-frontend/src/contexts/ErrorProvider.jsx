// /src/contexts/ErrorProvider.js
import React, { useState } from "react";
import ErrorContext from "./ErrorContext";

const ErrorProvider = ({ children }) => {
    // error / warning могут быть строками, объектами, чем угодно
    const [error, setError] = useState(null);
    const [warning, setWarning] = useState(null);

    // Показываем ошибку
    const showError = (msg) => {
        setError(msg);
        setWarning(null); // сбрасываем warning, если хотим показывать только одно сообщение
    };

    // Показываем предупреждение
    const showWarning = (msg) => {
        setWarning(msg);
        setError(null); // сбрасываем error
    };

    // Очищаем сообщения
    const clearMessage = () => {
        setError(null);
        setWarning(null);
    };

    return (
        <ErrorContext.Provider
            value={{
                error,
                warning,
                showError,
                showWarning,
                clearMessage,
            }}
        >
            {children}
        </ErrorContext.Provider>
    );
};

export default ErrorProvider;
