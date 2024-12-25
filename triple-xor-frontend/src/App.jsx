// src/App.js
import React from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "./styles/App.css";
import "./styles/Theme.css"; // Важный файл со всеми темами
import ThemeProvider from "./contexts/ThemeProvider";

import ErrorProvider from "./contexts/ErrorProvider";
import ErrorModal from "./components/ErrorModal";
import InitializeCard from "./components/InitializeCard";
import TrainCard from "./components/TrainCard";
import TestCard from "./components/TestCard";
import ParametersCard from "./components/ParametersCard";
import ThemeToggler from "./components/ThemeToggler";

function App() {
    const apiBase = "https://localhost:7156/api/NeuralNetwork";

    return (
        <ErrorProvider>
            <ThemeProvider>
                <ErrorModal />
                <div className="app-container">
                    <div className="d-flex align-items-center mb-4">
                        <h1>Neural Network App ⚡</h1>
                        <ThemeToggler />
                    </div>

                    <InitializeCard apiBase={apiBase} />
                    <TrainCard apiBase={apiBase} />
                    <TestCard apiBase={apiBase} />
                    <ParametersCard apiBase={apiBase} />
                </div>
            </ThemeProvider>
        </ErrorProvider>
    );
}

export default App;
