// /src/components/InitializeCard.js
import React, { useState, useContext } from "react";
import ThemeContext from "../contexts/ThemeContext";
import ErrorContext from "../contexts/ErrorContext";

function InitializeCard({ apiBase }) {
    const [inputSize, setInputSize] = useState(8);
    const [hiddenNeurons, setHiddenNeurons] = useState(3);
    const [outputNeurons, setOutputNeurons] = useState(1);
    const [learningRate, setLearningRate] = useState(0.1);

    const { showError, showWarning } = useContext(ErrorContext);
    const [isInitializing, setIsInitializing] = useState(false);

    const { theme } = useContext(ThemeContext);

    const initializeNetwork = async () => {
        if (inputSize <= 0) {
            showWarning("Input Size не может быть меньше или равно 0");
            return;
        }
        setIsInitializing(true);
        try {
            const url = `${apiBase}/Initialize?inputSize=${inputSize}`;
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error("Не удалось инициализировать сеть на сервере");
            }
            const message = await response.text();
            alert(message);
        } catch (error) {
            // Показываем окно ошибки
            showError(error.message || "Неизвестная ошибка");
        } finally {
            setIsInitializing(false);
        }
    };

    return (
        <div
            className={`card mb-4 ${
                theme === "dark" ? "bg-secondary text-light" : "bg-light text-dark"
            }`}
        >
            <div className="card-header d-flex justify-content-between align-items-center">
                <span>1. Initialize Network</span>
                {isInitializing && (
                    <div className="spinner-border spinner-border-sm text-light" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                )}
            </div>
            <div className="card-body">
                <div className="row mb-3">
                    <div className="col-3">
                        <label>Input Size:</label>
                        <input
                            type="number"
                            className="form-control"
                            value={inputSize}
                            onChange={(e) => setInputSize(Number(e.target.value))}
                        />
                    </div>
                    <div className="col-3">
                        <label>Hidden Neurons:</label>
                        <input
                            type="number"
                            className="form-control"
                            value={hiddenNeurons}
                            onChange={(e) => setHiddenNeurons(Number(e.target.value))}
                        />
                    </div>
                    <div className="col-3">
                        <label>Output Neurons:</label>
                        <input
                            type="number"
                            className="form-control"
                            value={outputNeurons}
                            onChange={(e) => setOutputNeurons(Number(e.target.value))}
                        />
                    </div>
                    <div className="col-3">
                        <label>Learning Rate:</label>
                        <input
                            type="number"
                            className="form-control"
                            step="0.01"
                            value={learningRate}
                            onChange={(e) => setLearningRate(Number(e.target.value))}
                        />
                    </div>
                </div>

                <button
                    className="btn btn-primary"
                    onClick={initializeNetwork}
                    disabled={isInitializing}
                >
                    {isInitializing ? (
                        <>
                            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                            &nbsp;Инициализация...
                        </>
                    ) : (
                        "Initialize Network"
                    )}
                </button>
            </div>
        </div>
    );
}

export default InitializeCard;
