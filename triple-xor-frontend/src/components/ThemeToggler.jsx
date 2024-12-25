// src/components/ThemeToggler.js
import React, { useContext } from "react";
import ThemeContext from "../contexts/ThemeContext";

const ThemeToggler = () => {
    const { theme, toggleTheme } = useContext(ThemeContext);

    return (
        <button
            className="theme-toggler"
            onClick={toggleTheme}
            title="Сменить тему"
        >
            {theme === "dark" ? "🌞 Светлая" : "🌚 Тёмная"}
        </button>
    );
};

export default ThemeToggler;
