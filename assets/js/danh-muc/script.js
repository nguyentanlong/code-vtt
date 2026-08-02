/* ---------- Sidebar nav icons ---------- */
const icons = {
  home: '<path d="M3 11.5 12 4l9 7.5"/><path d="M5 10v10h14V10"/>',
  folder: '<path d="M3 7a2 2 0 0 1 2-2h4l2 2h8a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2Z"/>',
  dollar: '<circle cx="12" cy="12" r="9"/><path d="M12 7v10M15 9.5c0-1.4-1.3-2.5-3-2.5s-3 1-3 2.3c0 3 6 1.5 6 4.5 0 1.4-1.4 2.4-3 2.4s-3-1-3-2.4"/>',
  truck: '<rect x="1" y="7" width="13" height="9"/><path d="M14 10h4l3 3v3h-7z"/><circle cx="6" cy="18" r="1.5"/><circle cx="17" cy="18" r="1.5"/>',
  users: '<circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M15 20c.2-2.6 1.8-4.6 4-5.3"/>',
  pencil: '<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z"/>',
  bank: '<path d="M3 21h18"/><path d="M4 21V10M20 21V10M8 21V10M16 21V10M2 10l10-6 10 6"/>',
  chart: '<path d="M3 3v18h18"/><path d="M7 15l4-5 3 3 5-7"/>',
  check: '<circle cx="12" cy="12" r="9"/><path d="m8 12 3 3 5-6"/>',
  users2: '<circle cx="8" cy="8" r="3"/><circle cx="16" cy="8" r="3"/><path d="M2 20c0-3 2.7-5.5 6-5.5s6 2.5 6 5.5"/><path d="M13 14.8c1-.5 2-.8 3-.8 3.3 0 6 2.5 6 5.5"/>'
};
document.querySelectorAll('.nav-item').forEach(item => {
  const key = item.dataset.icon;
  const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
  svg.setAttribute('viewBox', '0 0 24 24');
  svg.setAttribute('fill', 'none');
  svg.setAttribute('stroke', 'currentColor');
  svg.setAttribute('stroke-width', '2');
  svg.setAttribute('stroke-linecap', 'round');
  svg.setAttribute('stroke-linejoin', 'round');
  svg.innerHTML = icons[key] || '';
  item.prepend(svg);
  item.addEventListener('click', (e) => {
    const link = item.querySelector('a');
    if (link && link.getAttribute('href')) {
      e.preventDefault();
      window.location.href = link.getAttribute('href');
      return;
    }
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    item.classList.add('active');
    showPage(item.dataset.page);
    if (window.innerWidth <= 1024) closeSidebar();
  });
});

/* ---------- Chuyển nội dung khu vực giữa theo mục sidebar đang chọn ---------- */
function showPage(pageKey) {
  if (!pageKey) return;
  document.querySelectorAll('#pageContainer .page').forEach(sec => {
    sec.style.display = 'none';
  });
  const target = document.getElementById('page-' + pageKey);
  if (target) {
    target.style.display = 'block';
    target.scrollIntoView({ block: 'start' });
  }
}

/* ---------- Bar + line chart ---------- */
(function drawBarChart() {
  const svg = document.getElementById('barChart');
  if (!svg) return; // trang này không có chart doanh thu, bỏ qua

  const labels = ['12/2024', '01/2025', '02/2025', '03/2025', '04/2025', '05/2025'];
  const revenue = [10.5, 8.3, 9.6, 9.0, 15.8, 17.0]; // in B
  const profit = [10.2, 8.0, 7.8, 9.0, 12.5, 12.8]; // for line, arbitrary path matching visual
  const W = 640, H = 240, padL = 36, padB = 26, padT = 10;
  const chartW = W - padL - 10, chartH = H - padB - padT;
  const maxY = 20;
  const barW = 34, gap = (chartW - barW * labels.length) / (labels.length + 1);

  let svgContent = '';
  [0, 5, 10, 15, 20].forEach(v => {
    const y = padT + chartH - (v / maxY) * chartH;
    svgContent += `<line x1="${padL}" y1="${y}" x2="${W - 10}" y2="${y}" stroke="#eef1f6" stroke-width="1"/>`;
    svgContent += `<text x="${padL - 10}" y="${y + 4}" font-size="11" fill="#94a3b8" text-anchor="end">${v === 0 ? '0' : v + 'B'}</text>`;
  });

  const points = [];
  labels.forEach((lab, i) => {
    const x = padL + gap + i * (barW + gap);
    const bh = (revenue[i] / maxY) * chartH;
    const y = padT + chartH - bh;
    svgContent += `<rect x="${x}" y="${y}" width="${barW}" height="${bh}" rx="4" fill="#3b82f6"/>`;
    svgContent += `<text x="${x + barW / 2}" y="${H - 6}" font-size="11" fill="#94a3b8" text-anchor="middle">${lab}</text>`;
    const py = padT + chartH - (profit[i] / maxY) * chartH;
    points.push([x + barW / 2, py]);
  });

  let path = `M ${points[0][0]} ${points[0][1]}`;
  for (let i = 1; i < points.length; i++) { path += ` L ${points[i][0]} ${points[i][1]}`; }
  svgContent += `<path d="${path}" fill="none" stroke="#f97316" stroke-width="2.5"/>`;
  points.forEach(p => { svgContent += `<circle cx="${p[0]}" cy="${p[1]}" r="3.5" fill="#f97316"/>`; });

  svg.innerHTML = svgContent;
})();

/* ---------- Donut chart ---------- */
(function drawDonut() {
  const svg = document.getElementById('donutChart');
  if (!svg) return; // trang này không có donut chart, bỏ qua

  const data = [
    { v: 48.6, color: '#16a34a' },
    { v: 28.9, color: '#f59e0b' },
    { v: 15.0, color: '#f97316' },
    { v: 7.5, color: '#dc2626' }
  ];
  const cx = 90, cy = 90, r = 68, stroke = 26;
  const circ = 2 * Math.PI * r;
  let offset = 0, content = '';
  data.forEach(d => {
    const len = (d.v / 100) * circ;
    content += `<circle cx="${cx}" cy="${cy}" r="${r}" fill="none" stroke="${d.color}" stroke-width="${stroke}"
        stroke-dasharray="${len} ${circ - len}" stroke-dashoffset="${-offset}" transform="rotate(-90 ${cx} ${cy})"/>`;
    offset += len;
  });
  content += `<circle cx="${cx}" cy="${cy}" r="${r - stroke / 2 - 2}" fill="#fff"/>`;
  content += `<text x="${cx}" y="${cy - 4}" text-anchor="middle" font-size="12" fill="#64748b">Tổng</text>`;
  content += `<text x="${cx}" y="${cy + 16}" text-anchor="middle" font-size="17" font-weight="700" fill="#1e293b">4.320M</text>`;
  svg.innerHTML = content;
})();

/* ---------- Mini bars in chat ---------- */
(function drawMiniBars() {
  const el = document.getElementById('miniBars');
  if (!el) return;
  const vals = [30, 45, 25, 55, 70, 85];
  el.innerHTML = vals.map(v => `<span style="height:${v}%"></span>`).join('');
})();

/* ---------- Suggest buttons -> add message ---------- */
document.querySelectorAll('.suggest-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    const body = document.getElementById('chatBody');
    const userMsg = document.createElement('div');
    userMsg.className = 'msg-row user';
    userMsg.innerHTML = `<div class="bubble">${btn.textContent}</div>`;
    body.appendChild(userMsg);
    body.scrollTop = body.scrollHeight;
  });
});

/* ---------- Send button ---------- */
const sendButton = document.querySelector('.send-btn');
const chatInput = document.querySelector('.chat-input-row input');
if (sendButton) {
  sendButton.addEventListener('click', sendMsg);
}
if (chatInput) {
  chatInput.addEventListener('keydown', e => {
    if (e.key === 'Enter') sendMsg();
  });
}
function sendMsg() {
  const input = document.querySelector('.chat-input-row input');
  if (!input) return;
  const text = input.value.trim();
  if (!text) return;
  const body = document.getElementById('chatBody');
  if (!body) return;
  const userMsg = document.createElement('div');
  userMsg.className = 'msg-row user';
  userMsg.innerHTML = `<div class="bubble">${text}</div>`;
  body.appendChild(userMsg);
  input.value = '';
  body.scrollTop = body.scrollHeight;
}

/* ---------- Sidebar mobile toggle ---------- */
const sidebar = document.getElementById('sidebar');
const sidebarOverlay = document.getElementById('sidebarOverlay');
const menuToggle = document.getElementById('menuToggle');

function openSidebar() {
  if (sidebar) sidebar.classList.add('open');
  if (sidebarOverlay) sidebarOverlay.classList.add('open');
}
function closeSidebar() {
  if (sidebar) sidebar.classList.remove('open');
  if (sidebarOverlay) sidebarOverlay.classList.remove('open');
}
if (menuToggle) menuToggle.addEventListener('click', openSidebar);
if (sidebarOverlay) sidebarOverlay.addEventListener('click', closeSidebar);

/* ---------- FlowChat: thu gọn / mở rộng ---------- */
const mainWrapper = document.getElementById('mainWrapper');
const chatOverlay = document.getElementById('chatOverlay');
const chatFab = document.getElementById('chatFab');
const chatClose = document.getElementById('chatClose');

function openChat() {
  if (mainWrapper) mainWrapper.classList.add('chat-open');
  if (chatOverlay) chatOverlay.classList.add('open');
  if (chatFab) chatFab.classList.add('hidden');
}
function closeChat() {
  if (mainWrapper) mainWrapper.classList.remove('chat-open');
  if (chatOverlay) chatOverlay.classList.remove('open');
  if (chatFab) chatFab.classList.remove('hidden');
}
if (chatFab) chatFab.addEventListener('click', openChat);
if (chatClose) chatClose.addEventListener('click', closeChat);
if (chatOverlay) chatOverlay.addEventListener('click', closeChat);

// Mặc định: FlowChat thu gọn để không chiếm không gian
closeChat();
