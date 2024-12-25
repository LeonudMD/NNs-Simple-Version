import React, { useContext } from "react";
import ErrorContext from "../contexts/ErrorContext";

const ErrorModal = () => {
    const { error, warning, clearMessage } = useContext(ErrorContext);

    if (!error && !warning) return null;

    const isError = !!error;
    const title = isError ? "Ошибка" : "Предупреждение";
    const messageText = isError ? error : warning;

    return (
        <>
            <div
                style={{
                    position: "fixed",
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    backgroundColor: "rgba(0,0,0,0.5)",
                    zIndex: 9998,
                    animation: "fadeIn 0.3s ease-in-out",
                }}
                onClick={clearMessage}
            ></div>
            <div
                style={{
                    position: "fixed",
                    top: "50%",
                    left: "50%",
                    transform: "translate(-50%, -50%)",
                    backgroundColor: "#fff",
                    color: "#333",
                    width: "400px",
                    borderRadius: "12px",
                    boxShadow: "0 8px 20px rgba(0, 0, 0, 0.2)",
                    zIndex: 9999,
                    overflow: "hidden",
                    animation: "slideDown 0.4s ease-in-out",
                }}
            >
                <div
                    style={{
                        backgroundColor: isError ? "#ff5555" : "#ffaa00",
                        color: "#fff",
                        padding: "1rem",
                        textAlign: "center",
                        fontWeight: "bold",
                    }}
                >
                    {title}
                </div>
                <div style={{ padding: "1rem", textAlign: "center" }}>
                    <p>{messageText}</p>
                </div>
                <div
                    style={{
                        display: "flex",
                        justifyContent: "center",
                        padding: "1rem",
                    }}
                >
                    <button
                        onClick={clearMessage}
                        style={{
                            backgroundColor: isError ? "#ff5555" : "#ffaa00",
                            border: "none",
                            color: "#fff",
                            padding: "0.5rem 1rem",
                            borderRadius: "6px",
                            cursor: "pointer",
                            transition: "all 0.3s",
                        }}
                        onMouseOver={(e) =>
                            (e.target.style.backgroundColor = isError
                                ? "#ff3333"
                                : "#ff8800")
                        }
                        onMouseOut={(e) =>
                            (e.target.style.backgroundColor = isError
                                ? "#ff5555"
                                : "#ffaa00")
                        }
                    >
                        Закрыть
                    </button>
                </div>
            </div>
            <style>
                {`
                    @keyframes fadeIn {
                        from { opacity: 0; }
                        to { opacity: 1; }
                    }

                    @keyframes slideDown {
                        from { transform: translate(-50%, -60%); }
                        to { transform: translate(-50%, -50%); }
                    }
                `}
            </style>
        </>
    );
};

export default ErrorModal;
