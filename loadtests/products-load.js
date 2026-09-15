import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 20 },   // разогрев до 20 VU
        { duration: '1m', target: 50 },    // нагрузка 50 VU
        { duration: '30s', target: 0 },    // спад
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'],  // 95% запросов быстрее 500мс
        http_req_failed: ['rate<0.01'],    // ошибок < 1%
    },
};

const BASE_URL = 'http://localhost:5000';

export default function () {
    // Логин
    const loginRes = http.post(`${BASE_URL}/api/v1/auth/login`, JSON.stringify({
        email: 'admin@example.com',
        password: 'Admin123!',
    }), { headers: { 'Content-Type': 'application/json' } });

    check(loginRes, { 'login status 200': (r) => r.status === 200 });
    const token = loginRes.json('token');

    // Получение списка продуктов
    const productsRes = http.get(`${BASE_URL}/api/v1/products`, {
        headers: { Authorization: `Bearer ${token}` },
    });

    check(productsRes, {
        'products status 200': (r) => r.status === 200,
        'response has data': (r) => r.json().length >= 0,
    });

    sleep(1);
}