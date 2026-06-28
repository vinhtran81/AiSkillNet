Đọc file `.agents/skills/senior-integration/SKILL.md` và áp dụng integration patterns cho task sau.

Nguyên tắc bất biến: Email và Zalo KHÔNG gọi trực tiếp trong HTTP handler — luôn enqueue qua Hangfire.

$ARGUMENTS
