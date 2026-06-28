import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 10 },
        { duration: '1m',  target: 50 },
        { duration: '30s', target: 0 },
    ],
    thresholds: {
        'http_req_duration{endpoint:service-package}': ['p95<500'],
        'http_req_duration{endpoint:register-get}':    ['p95<500'],
        'http_req_duration{endpoint:admin-pending}':   ['p95<800'],
        'http_req_failed': ['rate<0.01'],
    },
};

export default function () {
    const base = __ENV.BASE_URL || 'http://localhost:5051';
    const cookie = __ENV.AUTH_COOKIE || '';

    const pkg = http.get(`${base}/ServicePackage`, { tags: { endpoint: 'service-package' } });
    check(pkg, { 'packages page 200': r => r.status === 200 });

    sleep(1);

    const form = http.get(`${base}/Membership/Register`, {
        tags: { endpoint: 'register-get' },
        headers: cookie ? { Cookie: cookie } : {}
    });
    check(form, { 'register redirects or 200': r => r.status === 200 || r.status === 302 });

    sleep(2);
}

// Chạy: k6 run --env BASE_URL=http://localhost:5051 tests/k6/membership-registration-load.js
