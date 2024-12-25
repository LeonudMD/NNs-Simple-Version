// /src/contexts/ErrorContext.js
import { createContext } from "react";

const ErrorContext = createContext({
    error: null,
    warning: null,
    showError: () => {},
    showWarning: () => {},
    clearMessage: () => {},
});

export default ErrorContext;
