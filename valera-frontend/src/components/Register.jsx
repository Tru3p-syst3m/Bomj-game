import React, { useState } from 'react';
import { register, login } from '../services/api';
import { Link, useNavigate } from 'react-router-dom';
import './Register.css';

const Register = ({ setIsAuthenticated }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            await register({ email, password, username });
            // После успешной регистрации логиним и перенаправляем
            await login({ email, password });
            setIsAuthenticated(true);
            window.dispatchEvent(new Event('authChange'));
            navigate('/login');
        } catch (err) {
            setError('Ошибка при регистрации');
        }
    };

    return (
        <div className="register-container">
            <div className="register-form">
                <h2>Регистрация</h2>
                {error && <div className="error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="username">Имя пользователя:</label>
                        <input
                            type="text"
                            id="username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="email">Email:</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Пароль:</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn-register">Зарегистрироваться</button>
                </form>
                <div className="auth-link">
                    <p>Уже есть аккаунт? <Link to="/login">Войти</Link></p>
                </div>
            </div>
        </div>
    );
};

export default Register;
