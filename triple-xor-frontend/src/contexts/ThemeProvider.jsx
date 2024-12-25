// src/contexts/ThemeProvider.js
import React, { useState, useEffect } from "react";
import ThemeContext from "./ThemeContext";

const ThemeProvider = ({ children }) => {
    const [theme, setTheme] = useState("light");

    // Проверяем localStorage при первом рендере
    useEffect(() => {
        const storedTheme = localStorage.getItem("app-theme");
        if (storedTheme) {
            setTheme(storedTheme);
        }
    }, []);

    // Меняем data-theme для <html> и обновляем localStorage
    useEffect(() => {
        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem("app-theme", theme);
    }, [theme]);

    const toggleTheme = () => {
        setTheme((prev) => (prev === "dark" ? "light" : "dark"));
    };

    return (
        <ThemeContext.Provider value={{ theme, toggleTheme }}>
            {children}
        </ThemeContext.Provider>
    );
};

export default ThemeProvider;
