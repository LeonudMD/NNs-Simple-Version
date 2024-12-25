// /src/components/TestCard.js
import React, { useState, useContext } from "react";
import ThemeContext from "../contexts/ThemeContext";

function TestCard({ apiBase }) {
    const [testResults, setTestResults] = useState([]);
    const [testingStats, setTestingStats] = useState(null);
    const [isTesting, setIsTesting] = useState(false);

    const { theme } = useContext(ThemeContext);

    const testNetwork = async () => {
        setIsTesting(true);
        const startTime = Date.now();
        try {
            const response = await fetch(`${apiBase}/Test`);
            const data = await response.json();
            setTestResults(data);

            const endTime = Date.now();
            const timeTaken = ((endTime - startTime) / 1000).toFixed(2);
            const totalTests = data.length;
            const correctTests = data.filter((res) => res.expected === res.predicted).length;
            const accuracy = totalTests > 0 ? ((correctTests / totalTests) * 100).toFixed(2) : "0.00";

            setTestingStats({
                total: totalTests,
                correct: correctTests,
                accuracy: accuracy,
                time: timeTaken,
            });
        } catch (error) {
            console.error(error);
            setTestResults([]);
            setTestingStats(null);
            alert("Ошибка тестирования сети.");
        } finally {
            setIsTesting(false);
        }
    };

    const clearTestResults = () => {
        setTestResults([]);
        setTestingStats(null);
    };

    return (
        <div
            className={`card mb-4 ${
                theme === "dark" ? "bg-secondary text-light" : "bg-light text-dark"
            }`}
        >
            <div className="card-header d-flex justify-content-between align-items-center">
                <span>3. Test Network</span>
                {isTesting && (
                    <div className="spinner-border spinner-border-sm text-light" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                )}
            </div>
            <div className="card-body">
                <button className="btn btn-info mb-3" onClick={testNetwork} disabled={isTesting}>
                    {isTesting ? (
                        <>
                            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                            &nbsp;Тестирование...
                        </>
                    ) : (
                        "Test"
                    )}
                </button>
                <button
                    className="btn btn-outline-secondary mb-3 ms-2"
                    onClick={clearTestResults}
                    disabled={testResults.length === 0 && !testingStats}
                >
                    Очистить
                </button>

                {testingStats && (
                    <div className="statistic-card">
                        <h5>Testing Statistics</h5>
                        <p><strong>Total Tests:</strong> {testingStats.total}</p>
                        <p><strong>Correct Predictions:</strong> {testingStats.correct}</p>
                        <p><strong>Accuracy:</strong> {testingStats.accuracy}%</p>
                        <p><strong>Time Taken:</strong> {testingStats.time} seconds</p>
                    </div>
                )}

                <div style={{ maxHeight: "300px", overflowY: "auto" }}>
                    {testResults.length > 0 ? (
                        <table className="table table-bordered table-dark">
                            <thead>
                            <tr>
                                <th>Inputs</th>
                                <th>Expected</th>
                                <th>Predicted</th>
                            </tr>
                            </thead>
                            <tbody>
                            {testResults.map((res, index) => (
                                <tr
                                    key={index}
                                    className={res.expected === res.predicted ? "match" : "mismatch"}
                                >
                                    <td>[{res.inputs.join(", ")}]</td>
                                    <td>{res.expected}</td>
                                    <td>{res.predicted}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    ) : (
                        <p>Нет результатов тестирования</p>
                    )}
                </div>
            </div>
        </div>
    );
}

export default TestCard;
