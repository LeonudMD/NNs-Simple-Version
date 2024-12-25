// /src/components/TrainCard.js
import React, { useState, useContext } from "react";
import ThemeContext from "../contexts/ThemeContext";

function TrainCard({ apiBase }) {
    const [epochs, setEpochs] = useState(10000);
    const [threshold, setThreshold] = useState(0.001);
    const [trainResult, setTrainResult] = useState("");
    const [trainingStats, setTrainingStats] = useState(null);
    const [isTraining, setIsTraining] = useState(false);

    const { theme } = useContext(ThemeContext);

    const trainNetwork = async () => {
        setIsTraining(true);
        const startTime = Date.now();
        try {
            const response = await fetch(`${apiBase}/Train`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ epochs, threshold }),
            });
            const data = await response.text();
            const endTime = Date.now();
            const timeTaken = ((endTime - startTime) / 1000).toFixed(2);

            setTrainResult(data);
            setTrainingStats({
                epochs: epochs,
                threshold: threshold,
                time: timeTaken,
            });
        } catch (error) {
            console.error(error);
            setTrainResult("Ошибка при обучении сети.");
            setTrainingStats(null);
        } finally {
            setIsTraining(false);
        }
    };

    const clearTrainResult = () => {
        setTrainResult("");
        setTrainingStats(null);
    };

    return (
        <div
            className={`card mb-4 ${
                theme === "dark" ? "bg-secondary text-light" : "bg-light text-dark"
            }`}
        >
            <div className="card-header d-flex justify-content-between align-items-center">
                <span>2. Train Network</span>
                {isTraining && (
                    <div className="spinner-border spinner-border-sm text-light" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                )}
            </div>
            <div className="card-body">
                <div className="row mb-3">
                    <div className="col-6">
                        <label>Epochs:</label>
                        <input
                            type="number"
                            className="form-control"
                            value={epochs}
                            onChange={(e) => setEpochs(Number(e.target.value))}
                        />
                    </div>
                    <div className="col-6">
                        <label>Threshold:</label>
                        <input
                            type="number"
                            className="form-control"
                            step="0.0001"
                            value={threshold}
                            onChange={(e) => setThreshold(Number(e.target.value))}
                        />
                    </div>
                </div>

                <button className="btn btn-success" onClick={trainNetwork} disabled={isTraining}>
                    {isTraining ? (
                        <>
                            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                            &nbsp;Обучение...
                        </>
                    ) : (
                        "Train"
                    )}
                </button>
                <button
                    className="btn btn-outline-secondary ms-2"
                    onClick={clearTrainResult}
                    disabled={!trainResult && !trainingStats}
                >
                    Очистить
                </button>

                <div className="mt-3">
                    <strong>Train Result:</strong> {trainResult || "Нет результатов"}
                </div>

                {trainingStats && (
                    <div className="mt-3 statistic-card">
                        <h5>Training Statistics</h5>
                        <p><strong>Epochs:</strong> {trainingStats.epochs}</p>
                        <p><strong>Threshold:</strong> {trainingStats.threshold}</p>
                        <p><strong>Time Taken:</strong> {trainingStats.time} seconds</p>
                    </div>
                )}
            </div>
        </div>
    );
}

export default TrainCard;
