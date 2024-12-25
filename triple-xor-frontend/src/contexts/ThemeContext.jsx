// src/contexts/ThemeContext.js
import { createContext } from "react";

const ThemeContext = createContext({
    theme: "light",       // Тема по умолчанию
    toggleTheme: () => {},
});

export default ThemeContext;
