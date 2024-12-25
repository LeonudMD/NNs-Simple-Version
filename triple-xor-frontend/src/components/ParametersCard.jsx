// /src/components/ParametersCard.js
import React, { useState, useContext } from "react";
import ThemeContext from "../contexts/ThemeContext";

function ParametersCard({ apiBase }) {
    const [parameters, setParameters] = useState([]);
    const [isGettingParameters, setIsGettingParameters] = useState(false);

    const { theme } = useContext(ThemeContext);

    const getParameters = async () => {
        setIsGettingParameters(true);
        try {
            const response = await fetch(`${apiBase}/Parameters`);
            const data = await response.json();
            setParameters(data);
        } catch (error) {
            console.error(error);
            setParameters([]);
            alert("Ошибка получения параметров сети.");
        } finally {
            setIsGettingParameters(false);
        }
    };

    const clearParameters = () => setParameters([]);

    return (
        <div
            className={`card mb-4 ${
                theme === "dark" ? "bg-secondary text-light" : "bg-light text-dark"
            }`}
        >
            <div className="card-header d-flex justify-content-between align-items-center">
                <span>4. Network Parameters</span>
                {isGettingParameters && (
                    <div className="spinner-border spinner-border-sm text-light" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                )}
            </div>
            <div className="card-body">
                <button
                    className="btn btn-warning mb-3"
                    onClick={getParameters}
                    disabled={isGettingParameters}
                >
                    {isGettingParameters ? (
                        <>
                            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                            &nbsp;Получение...
                        </>
                    ) : (
                        "Get Parameters"
                    )}
                </button>
                <button
                    className="btn btn-outline-secondary mb-3 ms-2"
                    onClick={clearParameters}
                    disabled={parameters.length === 0}
                >
                    Очистить
                </button>

                <div style={{ maxHeight: "300px", overflowY: "auto" }}>
                    {parameters.length > 0 ? (
                        parameters.map((layer, i) => (
                            <div key={i} className="parameters-layer">
                                <h5>Layer {layer.layerIndex}</h5>
                                {layer.neurons.map((n, j) => (
                                    <div key={j} className="parameters-neuron">
                                        <p>
                                            <strong>Neuron {n.neuronIndex}</strong>
                                        </p>
                                        <p>Weights: [{n.weights.join(", ")}]</p>
                                        <p>Bias: {n.bias}</p>
                                    </div>
                                ))}
                            </div>
                        ))
                    ) : (
                        <p>Нет доступных параметров</p>
                    )}
                </div>
            </div>
        </div>
    );
}

export default ParametersCard;
