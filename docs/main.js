/* =============================================================
   CYBERWORLD OS — main.js
   Full OS interaction: boot, window manager, terminal, apps
   FLLC Enterprise IT Solutions // Midnight Neon Galaxy
   ============================================================= */

'use strict';

/* ─────────────────────────────────────────────────────────────
   BOOT SEQUENCE
───────────────────────────────────────────────────────────── */
const BOOT_MESSAGES = [
  '[  0.000]  FLLC CYBERWORLD OS v1.0.0 — MIDNIGHT NEON GALAXY',
  '[  0.012]  CPU: CyberCore-X 128-core @ 9.6GHz  ARCH: x86_CW64',
  '[  0.024]  RAM: 256 TB Holographic Neon Memory initialized',
  '[  0.041]  Quantum SSD Array: 4.0 PB — ONLINE',
  '[  0.055]  Loading kernel modules...',
  '[  0.102]  neon_display.ko            [  OK  ]',
  '[  0.134]  quantum_net.ko             [  OK  ]',
  '[  0.178]  osint_daemon.ko            [  OK  ]',
  '[  0.203]  soc_monitor.ko             [  OK  ]',
  '[  0.244]  cyberworld_engine.ko       [  OK  ]',
  '[  0.289]  fllc_core.ko               [  OK  ]',
  '[  0.312]  Mounting virtual filesystem: /cyberworld',
  '[  0.345]  Mounting OSINT database: /osint/db',
  '[  0.378]  Mounting threat intelligence feed: /soc/intel',
  '[  0.401]  Starting FLLC Enterprise services...',
  '[  0.441]  sshd         STARTED  ::1:22',
  '[  0.462]  threat-feed  STARTED  ::1:9001',
  '[  0.483]  soc-api      STARTED  ::1:8080',
  '[  0.507]  Initializing CyberWorld MMORPG engine...',
  '[  0.551]  Loading world zones: NeonCity, DarkWebDungeon, SOCCommand',
  '[  0.589]  Loading world zones: OSINTHub, CyberBazaar, GalaxyNexus',
  '[  0.624]  Spawning NPC agents (1,024)...',
  '[  0.671]  Connecting to Quantum Mesh Network...',
  '[  0.718]  Firewall: ACTIVE  |  IDS: ACTIVE  |  VPN: ACTIVE',
  '[  0.743]  Encryption: AES-4096-GCM — ALL CHANNELS SECURED',
  '[  0.789]  FLLC Operations Center: ONLINE',
  '[  0.812]  Welcome, Operative. CyberWorld OS is ready.',
];

let bootIndex = 0;
let bootProgress = 0;

function runBoot() {
  const logEl = document.getElementById('boot-log');
  const progressEl = document.getElementById('boot-progress');
  const enterEl = document.getElementById('boot-enter');

  function tick() {
    if (bootIndex < BOOT_MESSAGES.length) {
      const line = document.createElement('div');
      line.className = 'boot-log-line';
      line.textContent = BOOT_MESSAGES[bootIndex];
      logEl.appendChild(line);
      logEl.scrollTop = logEl.scrollHeight;
      bootIndex++;
      bootProgress = Math.round((bootIndex / BOOT_MESSAGES.length) * 100);
      progressEl.style.width = bootProgress + '%';
      const delay = 30 + Math.random() * 60;
      setTimeout(tick, delay);
    } else {
      setTimeout(() => {
        enterEl.style.display = 'block';
      }, 400);
    }
  }
  tick();
}

function launchDesktop() {
  document.getElementById('boot-screen').style.display = 'none';
  const desktop = document.getElementById('desktop');
  desktop.style.display = 'block';
  startClock();
  startStarfield();
  startSOCFeed();
  // Auto-open game on first launch
  setTimeout(() => openWindow('game'), 600);
}

document.addEventListener('DOMContentLoaded', () => {
  runBoot();

  const bootEnter = document.getElementById('boot-enter');
  bootEnter.addEventListener('click', launchDesktop);
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && document.getElementById('boot-screen').style.display !== 'none') {
      if (bootEnter.style.display !== 'none') launchDesktop();
    }
  });

  // Start menu toggle
  document.getElementById('start-btn').addEventListener('click', (e) => {
    e.stopPropagation();
    const menu = document.getElementById('start-menu');
    menu.style.display = menu.style.display === 'none' ? 'block' : 'none';
  });

  document.addEventListener('click', () => {
    document.getElementById('start-menu').style.display = 'none';
  });
});

/* ─────────────────────────────────────────────────────────────
   CLOCK
───────────────────────────────────────────────────────────── */
function startClock() {
  function tick() {
    const now = new Date();
    const h = String(now.getHours()).padStart(2, '0');
    const m = String(now.getMinutes()).padStart(2, '0');
    const s = String(now.getSeconds()).padStart(2, '0');
    const el = document.getElementById('taskbar-clock');
    if (el) el.textContent = `${h}:${m}:${s}`;
  }
  tick();
  setInterval(tick, 1000);
}

/* ─────────────────────────────────────────────────────────────
   STARFIELD CANVAS BACKGROUND
───────────────────────────────────────────────────────────── */
function startStarfield() {
  const canvas = document.createElement('canvas');
  canvas.id = 'starfield';
  canvas.style.cssText = 'position:fixed;inset:0;z-index:0;pointer-events:none;';
  document.getElementById('desktop').prepend(canvas);

  const ctx = canvas.getContext('2d');
  let stars = [];

  function resize() {
    canvas.width  = window.innerWidth;
    canvas.height = window.innerHeight;
    stars = Array.from({ length: 220 }, () => ({
      x: Math.random() * canvas.width,
      y: Math.random() * canvas.height,
      r: Math.random() * 1.5 + 0.3,
      a: Math.random(),
      da: (Math.random() - 0.5) * 0.008,
      color: ['#00ffcc','#7700ff','#ff00ff','#ffffff','#00ff44'][Math.floor(Math.random()*5)],
    }));
  }

  resize();
  window.addEventListener('resize', resize);

  function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    stars.forEach(s => {
      s.a += s.da;
      if (s.a <= 0 || s.a >= 1) s.da *= -1;
      ctx.globalAlpha = s.a * 0.8;
      ctx.fillStyle = s.color;
      ctx.beginPath();
      ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
      ctx.fill();
    });
    ctx.globalAlpha = 1;
    requestAnimationFrame(draw);
  }
  draw();
}

/* ─────────────────────────────────────────────────────────────
   WINDOW MANAGER
───────────────────────────────────────────────────────────── */
const windows = {};
let zCounter = 200;

const WINDOW_CONFIGS = {
  game:     { title: 'CYBERWORLD MMORPG',     icon: '⬡', w: 900, h: 600, buildFn: buildGameApp },
  terminal: { title: 'TERMINAL — CW//SHELL',  icon: '▶_', w: 700, h: 460, buildFn: buildTerminalApp },
  soc:      { title: 'SOC OPERATIONS CENTER', icon: '◈', w: 780, h: 520, buildFn: buildSOCApp },
  osint:    { title: 'OSINT TOOLKIT',          icon: '⊕', w: 700, h: 500, buildFn: buildOSINTApp },
  map:      { title: 'CYBERWORLD MAP',         icon: '⊞', w: 800, h: 540, buildFn: buildMapApp },
  avatar:   { title: 'AVATAR CREATOR',         icon: '⚙', w: 500, h: 540, buildFn: buildAvatarApp },
  about:    { title: 'ABOUT CYBERWORLD OS',    icon: 'ℹ', w: 580, h: 520, buildFn: buildAboutApp },
};

function openWindow(id) {
  document.getElementById('start-menu').style.display = 'none';

  if (windows[id]) {
    focusWindow(id);
    const win = document.getElementById(`win-${id}`);
    if (win) {
      win.style.display = 'flex';
      win.style.zIndex = ++zCounter;
    }
    return;
  }

  const cfg = WINDOW_CONFIGS[id];
  if (!cfg) return;

  const container = document.getElementById('windows-container');
  const vw = container.offsetWidth, vh = container.offsetHeight;
  const left = Math.max(20, Math.min(vw - cfg.w - 20, 60 + Object.keys(windows).length * 24));
  const top  = Math.max(20, Math.min(vh - cfg.h - 20, 40 + Object.keys(windows).length * 24));

  const win = document.createElement('div');
  win.className = 'cw-window focused';
  win.id = `win-${id}`;
  win.style.cssText = `left:${left}px;top:${top}px;width:${cfg.w}px;height:${cfg.h}px;z-index:${++zCounter};`;

  win.innerHTML = `
    <div class="cw-titlebar" data-win="${id}">
      <span class="cw-title-icon">${cfg.icon}</span>
      <span class="cw-title-text">${cfg.title}</span>
      <div class="cw-win-controls">
        <button class="cw-win-btn cw-btn-min"  onclick="minimizeWindow('${id}')" title="Minimize">—</button>
        <button class="cw-win-btn cw-btn-max"  onclick="maximizeWindow('${id}')" title="Maximize">□</button>
        <button class="cw-win-btn cw-btn-close" onclick="closeWindow('${id}')" title="Close">✕</button>
      </div>
    </div>
    <div class="cw-window-body" id="winbody-${id}"></div>
    <div class="cw-resize" data-win="${id}" title="Resize">◢</div>
  `;

  container.appendChild(win);
  windows[id] = { minimized: false, maxed: false, prevStyle: null };

  makeDraggable(win, win.querySelector('.cw-titlebar'));
  makeResizable(win, win.querySelector('.cw-resize'));
  win.addEventListener('mousedown', () => focusWindow(id));

  cfg.buildFn(document.getElementById(`winbody-${id}`));
  addTaskbarButton(id, cfg);
}

function focusWindow(id) {
  Object.keys(windows).forEach(wid => {
    const w = document.getElementById(`win-${wid}`);
    if (w) w.classList.remove('focused');
  });
  const win = document.getElementById(`win-${id}`);
  if (win) {
    win.classList.add('focused');
    win.style.zIndex = ++zCounter;
  }
  document.querySelectorAll('.taskbar-btn[data-win]').forEach(b => b.classList.remove('active'));
  const tb = document.querySelector(`.taskbar-btn[data-win="${id}"]`);
  if (tb) tb.classList.add('active');
}

function closeWindow(id) {
  const win = document.getElementById(`win-${id}`);
  if (win) win.remove();
  delete windows[id];
  const tb = document.querySelector(`.taskbar-btn[data-win="${id}"]`);
  if (tb) tb.remove();
}

function minimizeWindow(id) {
  const win = document.getElementById(`win-${id}`);
  if (!win) return;
  const w = windows[id];
  if (!w.minimized) {
    win.style.display = 'none';
    w.minimized = true;
  } else {
    win.style.display = 'flex';
    w.minimized = false;
    focusWindow(id);
  }
}

function maximizeWindow(id) {
  const win = document.getElementById(`win-${id}`);
  if (!win) return;
  const w = windows[id];
  if (!w.maxed) {
    w.prevStyle = { left: win.style.left, top: win.style.top, width: win.style.width, height: win.style.height };
    win.style.left = '0'; win.style.top = '0';
    win.style.width = '100%'; win.style.height = '100%';
    w.maxed = true;
  } else {
    Object.assign(win.style, w.prevStyle);
    w.maxed = false;
  }
}

function addTaskbarButton(id, cfg) {
  const bar = document.getElementById('taskbar-windows');
  const btn = document.createElement('button');
  btn.className = 'taskbar-btn active';
  btn.dataset.win = id;
  btn.textContent = `${cfg.icon} ${cfg.title.split('—')[0].trim()}`;
  btn.title = cfg.title;
  btn.addEventListener('click', () => {
    const w = windows[id];
    if (!w) return;
    if (w.minimized) {
      minimizeWindow(id); // un-minimize
    } else {
      const win = document.getElementById(`win-${id}`);
      if (win && win.classList.contains('focused')) {
        minimizeWindow(id);
      } else {
        focusWindow(id);
      }
    }
  });
  bar.appendChild(btn);
}

/* ─────────────────────────────────────────────────────────────
   DRAG & RESIZE
───────────────────────────────────────────────────────────── */
function makeDraggable(win, handle) {
  let dragging = false, ox = 0, oy = 0;
  handle.addEventListener('mousedown', e => {
    if (e.target.closest('.cw-win-controls')) return;
    dragging = true;
    ox = e.clientX - win.offsetLeft;
    oy = e.clientY - win.offsetTop;
    e.preventDefault();
  });
  document.addEventListener('mousemove', e => {
    if (!dragging) return;
    const container = document.getElementById('windows-container');
    const maxL = container.offsetWidth  - win.offsetWidth;
    const maxT = container.offsetHeight - win.offsetHeight;
    win.style.left = Math.max(0, Math.min(maxL, e.clientX - ox)) + 'px';
    win.style.top  = Math.max(0, Math.min(maxT, e.clientY - oy)) + 'px';
  });
  document.addEventListener('mouseup', () => { dragging = false; });
}

function makeResizable(win, handle) {
  let resizing = false, sx = 0, sy = 0, sw = 0, sh = 0;
  handle.addEventListener('mousedown', e => {
    resizing = true;
    sx = e.clientX; sy = e.clientY;
    sw = win.offsetWidth; sh = win.offsetHeight;
    e.preventDefault(); e.stopPropagation();
  });
  document.addEventListener('mousemove', e => {
    if (!resizing) return;
    win.style.width  = Math.max(320, sw + e.clientX - sx) + 'px';
    win.style.height = Math.max(200, sh + e.clientY - sy) + 'px';
  });
  document.addEventListener('mouseup', () => { resizing = false; });
}

/* ─────────────────────────────────────────────────────────────
   SHUTDOWN
───────────────────────────────────────────────────────────── */
function shutdown() {
  document.getElementById('start-menu').style.display = 'none';
  const desktop = document.getElementById('desktop');
  desktop.style.transition = 'opacity 1.5s';
  desktop.style.opacity = '0';
  setTimeout(() => {
    desktop.style.display = 'none';
    const sd = document.getElementById('shutdown-screen');
    sd.style.display = 'flex';
    sd.style.opacity = '0';
    sd.style.transition = 'opacity 1s';
    setTimeout(() => { sd.style.opacity = '1'; }, 50);
  }, 1500);
}

/* ─────────────────────────────────────────────────────────────
   TERMINAL APP
───────────────────────────────────────────────────────────── */
const TERMINAL_FS = {
  '/': ['home', 'soc', 'osint', 'cyberworld', 'fllc'],
  '/home': ['operative'],
  '/home/operative': ['README.txt', 'missions.log', '.bashrc'],
  '/soc': ['alerts.json', 'intel.db', 'playbooks'],
  '/osint': ['tools', 'reports', 'targets.enc'],
  '/cyberworld': ['zones', 'players.db', 'items.db', 'quests.db'],
  '/fllc': ['enterprise', 'ops', 'resources'],
};

const TERMINAL_FILES = {
  'README.txt': `CYBERWORLD OS v1.0.0
FLLC Enterprise IT Solutions
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Welcome, Operative.
This system is FLLC property.
Unauthorized access is a federal offense.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`,
  'missions.log': `[2026-03-19] Mission NEON-GHOST: COMPLETED
[2026-03-18] Mission CIPHER-RUN: IN PROGRESS
[2026-03-17] Mission DARK-FLUX: PENDING
[2026-03-16] Mission QUANTUM-BREACH: CLASSIFIED`,
  '.bashrc': `# CyberWorld Shell Config
export PATH=/cyberworld/bin:/fllc/tools:$PATH
export CW_ZONE="NeonCity"
alias ll='ls -la'
alias cw='cyberworld-launcher'
alias soc='soc-dashboard'`,
};

function buildTerminalApp(body) {
  body.innerHTML = `
    <div class="terminal-app">
      <div class="terminal-output" id="term-output"></div>
      <div class="terminal-input-row">
        <span class="terminal-prompt" id="term-prompt">operative@cyberworld:~$</span>
        <input class="terminal-input" id="term-input" type="text" autocomplete="off" spellcheck="false" autofocus />
      </div>
    </div>`;

  let cwd = '/home/operative';
  const history = [];
  let histIdx = -1;

  const output = body.querySelector('#term-output');
  const input  = body.querySelector('#term-input');
  const prompt = body.querySelector('#term-prompt');

  function print(text, cls = '') {
    const line = document.createElement('div');
    if (cls) line.className = cls;
    line.textContent = text;
    output.appendChild(line);
    output.scrollTop = output.scrollHeight;
  }

  print('CYBERWORLD OS v1.0.0 — CW//SHELL', 't-info');
  print('FLLC Enterprise IT Solutions', 't-info');
  print('Type "help" for available commands.', 't-dim');
  print('');

  function updatePrompt() {
    const short = cwd.replace('/home/operative', '~');
    prompt.textContent = `operative@cyberworld:${short}$`;
  }

  function resolve(path) {
    if (path.startsWith('/')) return path;
    if (path === '~') return '/home/operative';
    if (path.startsWith('~/')) return '/home/operative' + path.slice(1);
    const parts = cwd.split('/').filter(Boolean);
    path.split('/').forEach(p => {
      if (p === '..') { if (parts.length) parts.pop(); }
      else if (p && p !== '.') parts.push(p);
    });
    return '/' + parts.join('/');
  }

  const COMMANDS = {
    help() {
      [
        'Available commands:',
        '  help              — show this help',
        '  ls [path]         — list directory',
        '  cd <path>         — change directory',
        '  cat <file>        — read a file',
        '  pwd               — print working directory',
        '  whoami            — current user',
        '  hostname          — system hostname',
        '  uname             — system info',
        '  date              — current date/time',
        '  uptime            — system uptime',
        '  ps                — list processes',
        '  netstat           — network connections',
        '  ping <host>       — ping a host',
        '  nmap <target>     — scan ports (simulated)',
        '  whois <domain>    — WHOIS lookup (simulated)',
        '  cyberworld        — launch CyberWorld game',
        '  soc               — open SOC dashboard',
        '  osint             — open OSINT toolkit',
        '  fllc              — FLLC Enterprise info',
        '  matrix            — enter the matrix',
        '  clear             — clear terminal',
        '  exit              — close terminal',
      ].forEach(l => print(l, l.startsWith(' ') ? 't-dim' : 't-info'));
    },
    ls(args) {
      const path = args[0] ? resolve(args[0]) : cwd;
      const entries = TERMINAL_FS[path];
      if (!entries) { print(`ls: ${path}: No such directory`, 't-error'); return; }
      entries.forEach(e => print(e.includes('.') ? e : e + '/', e.includes('.') ? '' : 't-info'));
    },
    cd(args) {
      const path = args[0] ? resolve(args[0]) : '/home/operative';
      if (TERMINAL_FS[path]) { cwd = path; updatePrompt(); }
      else print(`cd: ${path}: No such directory`, 't-error');
    },
    cat(args) {
      if (!args[0]) { print('cat: missing operand', 't-error'); return; }
      const content = TERMINAL_FILES[args[0]];
      if (content) { content.split('\n').forEach(l => print(l)); }
      else print(`cat: ${args[0]}: No such file`, 't-error');
    },
    pwd() { print(cwd); },
    whoami() { print('operative'); },
    hostname() { print('cyberworld-os.fllc.net'); },
    uname(args) {
      if (args[0] === '-a') print('CyberWorldOS cwkernel-9.6.0-neon #1 SMP x86_CW64 GNU/CyberWorld');
      else print('CyberWorldOS');
    },
    date() { print(new Date().toString()); },
    uptime() {
      const s = Math.floor(performance.now() / 1000);
      print(`up ${Math.floor(s/3600)}h ${Math.floor((s%3600)/60)}m ${s%60}s,  1 user,  load average: 0.42, 0.17, 0.08`);
    },
    ps() {
      ['  PID  NAME                    STAT',
       '  001  systemd                 S',
       '  128  soc-monitor             S',
       '  256  threat-feed             S',
       '  384  cyberworld-engine       S',
       '  512  osint-daemon            S',
       '  640  neon-display            S',
       ' 1024  cw-shell                R',
      ].forEach(l => print(l, 't-dim'));
    },
    netstat() {
      ['Proto  Local Address           Foreign Address         State',
       'tcp    0.0.0.0:22              0.0.0.0:*               LISTEN',
       'tcp    0.0.0.0:443             0.0.0.0:*               LISTEN',
       'tcp    0.0.0.0:8080            0.0.0.0:*               LISTEN',
       'tcp    127.0.0.1:9001          0.0.0.0:*               LISTEN',
       'tcp    10.0.0.1:52441          185.220.101.9:443        ESTABLISHED',
      ].forEach(l => print(l, 't-dim'));
    },
    ping(args) {
      const host = args[0] || 'localhost';
      print(`PING ${host}: 56 bytes of data`, 't-info');
      for (let i = 0; i < 4; i++) {
        const ms = (Math.random() * 30 + 1).toFixed(3);
        print(`64 bytes from ${host}: icmp_seq=${i+1} ttl=64 time=${ms} ms`);
      }
      print(`--- ${host} ping statistics ---`, 't-dim');
      print(`4 packets transmitted, 4 received, 0% packet loss`, 't-success');
    },
    nmap(args) {
      const target = args[0] || '127.0.0.1';
      print(`Starting Nmap 7.94 — CyberWorld Edition`, 't-info');
      print(`Scanning ${target} ...`, 't-dim');
      const ports = [
        ['22/tcp',   'open', 'ssh'],
        ['80/tcp',   'open', 'http'],
        ['443/tcp',  'open', 'https'],
        ['8080/tcp', 'open', 'http-proxy'],
        ['9001/tcp', 'open', 'tor-orport'],
      ];
      ports.forEach(([port, state, svc]) => {
        print(`  ${port.padEnd(12)} ${state.padEnd(8)} ${svc}`, 't-success');
      });
      print(`5 open ports. Scan complete.`, 't-dim');
    },
    whois(args) {
      const domain = args[0] || 'fllc.net';
      [
        `Domain Name: ${domain.toUpperCase()}`,
        `Registrar: FLLC Enterprise Registrar`,
        `Status: clientTransferProhibited`,
        `Name Servers: ns1.fllc.net, ns2.fllc.net`,
        `DNSSEC: signedDelegation`,
        `Registrant: Furulie LLC`,
        `Tech Email: ops@fllc.net`,
      ].forEach(l => print(l, 't-info'));
    },
    cyberworld() { openWindow('game'); print('Launching CyberWorld MMORPG...', 't-success'); },
    soc()        { openWindow('soc');  print('Opening SOC Dashboard...', 't-success'); },
    osint()      { openWindow('osint'); print('Opening OSINT Toolkit...', 't-success'); },
    fllc() {
      [
        'FLLC ENTERPRISE IT SOLUTIONS',
        '══════════════════════════════════════',
        'Web:      https://www.fllc.net',
        'Ops:      https://www.fllc.net/ops',
        'Arsenal:  https://www.fllc.net/cyber-arsenal',
        'FuriosINT: https://www.fllc.net/furios-int',
        'Blog:     https://www.fllc.net/blog',
        'Resources: https://www.fllc.net/resources',
        '══════════════════════════════════════',
        'Services: Cybersecurity | OSINT | SOC | MMORPG Dev',
      ].forEach(l => print(l, 't-info'));
    },
    matrix() {
      print('Entering the Matrix...', 't-warn');
      let frames = 0;
      const chars = '0123456789ABCDEF';
      const iv = setInterval(() => {
        const row = Array.from({length: 60}, () => chars[Math.floor(Math.random()*chars.length)]).join(' ');
        print(row, 't-success');
        output.scrollTop = output.scrollHeight;
        if (++frames > 20) { clearInterval(iv); print('[Matrix sequence ended]', 't-dim'); }
      }, 80);
    },
    clear() { output.innerHTML = ''; },
    exit()  { closeWindow('terminal'); },
  };

  input.addEventListener('keydown', e => {
    if (e.key === 'Enter') {
      const raw = input.value.trim();
      if (!raw) return;
      history.unshift(raw);
      histIdx = -1;
      print(`${prompt.textContent} ${raw}`, 't-dim');
      input.value = '';
      const [cmd, ...args] = raw.split(/\s+/);
      const fn = COMMANDS[cmd.toLowerCase()];
      if (fn) fn(args);
      else print(`cw-shell: ${cmd}: command not found`, 't-error');
    } else if (e.key === 'ArrowUp') {
      histIdx = Math.min(histIdx + 1, history.length - 1);
      input.value = history[histIdx] || '';
    } else if (e.key === 'ArrowDown') {
      histIdx = Math.max(histIdx - 1, -1);
      input.value = histIdx < 0 ? '' : history[histIdx];
    }
  });

  // Focus terminal input when window is clicked
  body.addEventListener('click', () => input.focus());
  setTimeout(() => input.focus(), 100);
}

/* ─────────────────────────────────────────────────────────────
   SOC DASHBOARD APP
───────────────────────────────────────────────────────────── */
const SOC_ALERTS = [];
const SOC_ALERT_POOL = [
  ['crit', 'Ransomware beacon detected: C2 callback to 185.220.101.9'],
  ['high', 'Brute-force login attempt: admin@soc-portal (127 attempts)'],
  ['high', 'Lateral movement detected: 10.0.0.44 → 10.0.0.1 (SMB)'],
  ['med',  'Unusual DNS query volume from endpoint CW-PC-0042'],
  ['med',  'Port scan detected: 192.168.1.100 scanning /24 subnet'],
  ['low',  'SSL cert expiry warning: cert.fllc.net expires in 14 days'],
  ['crit', 'Exfiltration attempt: 4.2 GB upload to unknown IP'],
  ['high', 'Zero-day exploit attempt: CVE-2026-0042 — patched'],
  ['med',  'Phishing email quarantined: operative-alpha@fllc.net'],
  ['low',  'New device enrolled: CW-DEVICE-8819 — iOS 20.0'],
  ['high', 'Privilege escalation attempt: user cwoperative on HOST-007'],
  ['med',  'Anomalous login: geo-jump detected (NYC → Tokyo in 4 min)'],
  ['crit', 'DDoS spike: 2.3 Tbps — mitigation ACTIVE'],
  ['low',  'Vulnerability scan complete: 3 new CVEs flagged'],
  ['high', 'Malware sample isolated: NEON-RAT.v4 in quarantine'],
];

let socInterval = null;

function buildSOCApp(body) {
  body.innerHTML = `
    <div class="soc-app">
      <div class="soc-header">◈ FLLC SOC OPERATIONS CENTER — LIVE THREAT FEED</div>
      <div class="soc-grid" id="soc-stats"></div>
      <div class="soc-alert-list" id="soc-alert-list"></div>
    </div>`;

  const statsEl = body.querySelector('#soc-stats');
  const alertsEl = body.querySelector('#soc-alert-list');

  function updateStats() {
    const crit  = SOC_ALERTS.filter(a => a.sev === 'crit').length;
    const high  = SOC_ALERTS.filter(a => a.sev === 'high').length;
    const med   = SOC_ALERTS.filter(a => a.sev === 'med').length;
    const low   = SOC_ALERTS.filter(a => a.sev === 'low').length;
    statsEl.innerHTML = `
      <div class="soc-card"><div class="soc-card-title">CRITICAL</div><div class="soc-card-value red">${crit}</div></div>
      <div class="soc-card"><div class="soc-card-title">HIGH</div><div class="soc-card-value orange">${high}</div></div>
      <div class="soc-card"><div class="soc-card-title">MEDIUM</div><div class="soc-card-value violet">${med}</div></div>
      <div class="soc-card"><div class="soc-card-title">LOW</div><div class="soc-card-value cyan">${low}</div></div>
      <div class="soc-card"><div class="soc-card-title">TOTAL EVENTS</div><div class="soc-card-value green">${SOC_ALERTS.length}</div></div>
      <div class="soc-card"><div class="soc-card-title">UPTIME</div><div class="soc-card-value green">99.97%</div></div>`;
  }

  function renderAlerts() {
    alertsEl.innerHTML = '';
    [...SOC_ALERTS].reverse().slice(0, 30).forEach(a => {
      const item = document.createElement('div');
      item.className = 'soc-alert-item';
      const sevColor = { crit: '#ff003c', high: '#ff6600', med: '#ff00ff', low: '#00ff44' }[a.sev];
      item.innerHTML = `
        <div class="alert-dot ${a.sev}"></div>
        <span class="alert-time">${a.time}</span>
        <span class="alert-msg">${a.msg}</span>
        <span class="alert-sev" style="color:${sevColor}">${a.sev.toUpperCase()}</span>`;
      alertsEl.appendChild(item);
    });
  }

  function addAlert() {
    const pool = SOC_ALERT_POOL[Math.floor(Math.random() * SOC_ALERT_POOL.length)];
    const now = new Date();
    SOC_ALERTS.push({
      sev: pool[0], msg: pool[1],
      time: `${String(now.getHours()).padStart(2,'0')}:${String(now.getMinutes()).padStart(2,'0')}:${String(now.getSeconds()).padStart(2,'0')}`
    });
    updateStats();
    renderAlerts();
  }

  // Pre-populate with some alerts
  for (let i = 0; i < 8; i++) addAlert();

  if (!socInterval) {
    socInterval = setInterval(addAlert, 4000 + Math.random() * 3000);
  }
}

function startSOCFeed() {
  // Background feed (not tied to window)
  setInterval(() => {
    const pool = SOC_ALERT_POOL[Math.floor(Math.random() * SOC_ALERT_POOL.length)];
    const now = new Date();
    SOC_ALERTS.push({
      sev: pool[0], msg: pool[1],
      time: `${String(now.getHours()).padStart(2,'0')}:${String(now.getMinutes()).padStart(2,'0')}:${String(now.getSeconds()).padStart(2,'0')}`
    });
    if (SOC_ALERTS.length > 200) SOC_ALERTS.shift();
    const statusEl = document.getElementById('net-status');
    if (statusEl) {
      const last = SOC_ALERTS[SOC_ALERTS.length - 1];
      statusEl.textContent = last.sev === 'crit' ? '⚠ CRITICAL' : last.sev === 'high' ? '⚠ HIGH ALERT' : '◈ SECURE';
      statusEl.style.color = last.sev === 'crit' ? '#ff003c' : last.sev === 'high' ? '#ff6600' : '#00ff44';
    }
  }, 5000);
}

/* ─────────────────────────────────────────────────────────────
   OSINT TOOLKIT APP
───────────────────────────────────────────────────────────── */
const OSINT_TOOLS = [
  { icon: '🌐', name: 'Shodan',     fn: 'shodan' },
  { icon: '🔍', name: 'WHOIS',      fn: 'whois'  },
  { icon: '📡', name: 'DNS Recon',  fn: 'dns'    },
  { icon: '🗺', name: 'GeoIP',      fn: 'geo'    },
  { icon: '🔗', name: 'SpiderFoot', fn: 'spider' },
  { icon: '📧', name: 'Email Intel',fn: 'email'  },
  { icon: '🏴', name: 'Dark Web',   fn: 'dark'   },
  { icon: '🧬', name: 'Maltego',    fn: 'malt'   },
];

const OSINT_RESULTS = {
  shodan:  (q) => `Shodan search: "${q}"\n━━━━━━━━━━━━━━━━━━━━━━\nResults: 4,219 hosts\nTop country: United States (34%)\nTop port: 443 (HTTPS)\nTop org: Amazon AWS\nVulnerabilities flagged: 12\nCVEs: CVE-2026-0001, CVE-2025-9876`,
  whois:   (q) => `WHOIS: ${q}\n━━━━━━━━━━━━━━━━━━━━━━\nRegistrar: FLLC Registrar LLC\nCreated: 2022-01-01\nExpires: 2027-01-01\nStatus: Active\nName Servers: ns1.fllc.net`,
  dns:     (q) => `DNS Recon: ${q}\n━━━━━━━━━━━━━━━━━━━━━━\nA:     104.21.8.1\nAAAA:  2606:4700:3037::ac43:8601\nMX:    mail.${q} [10]\nTXT:   v=spf1 include:fllc.net ~all\nNS:    ns1.fllc.net, ns2.fllc.net`,
  geo:     (q) => `GeoIP: ${q}\n━━━━━━━━━━━━━━━━━━━━━━\nIP: 104.21.8.1\nCity: Ashburn\nState: Virginia\nCountry: United States\nISP: Cloudflare\nASN: AS13335\nThreat Score: LOW`,
  spider:  (q) => `SpiderFoot scan: "${q}"\n━━━━━━━━━━━━━━━━━━━━━━\nLinked domains: 8\nEmail addresses: 3\nPhone numbers: 1\nSocial profiles: 5\nLeaked credentials: 0\nData breach hits: 0`,
  email:   (q) => `Email Intel: "${q}"\n━━━━━━━━━━━━━━━━━━━━━━\nFormat: valid RFC 5322\nMX records: found\nDisposable: NO\nBreaches: 0\nLinked accounts: 3`,
  dark:    (q) => `Dark Web Monitor: "${q}"\n━━━━━━━━━━━━━━━━━━━━━━\nMentions found: 0\nPaste sites: 0\nForums: 0\nMarketplaces: 0\nStatus: CLEAN`,
  malt:    (q) => `Maltego transform: "${q}"\n━━━━━━━━━━━━━━━━━━━━━━\nEntities discovered: 47\nInfrastructure nodes: 12\nPerson nodes: 8\nOrg nodes: 6\nRelations mapped: 91`,
};

function buildOSINTApp(body) {
  body.innerHTML = `
    <div class="osint-app">
      <div class="osint-title">⊕ FLLC OSINT INTELLIGENCE TOOLKIT</div>
      <div class="osint-search-row">
        <input class="osint-input" id="osint-query" type="text" placeholder="Enter target (IP, domain, email, username...)" />
        <button class="osint-btn" onclick="osintSearch()">⊕ INTEL GATHER</button>
      </div>
      <div class="osint-tools-grid">
        ${OSINT_TOOLS.map(t => `
          <div class="osint-tool-card" onclick="osintRunTool('${t.fn}')">
            <div class="osint-tool-icon">${t.icon}</div>
            <div class="osint-tool-name">${t.name}</div>
          </div>`).join('')}
      </div>
      <div class="osint-results" id="osint-results">// Select a tool or enter a target query above...\n// All intelligence gathering is simulated for demonstration.</div>
    </div>`;
}

function osintSearch() {
  const q = document.getElementById('osint-query')?.value.trim() || 'unknown';
  const res = document.getElementById('osint-results');
  if (!res) return;
  res.textContent = `Running full OSINT sweep on: "${q}"\n━━━━━━━━━━━━━━━━━━━━━━\n`;
  Object.values(OSINT_RESULTS).forEach(fn => {
    res.textContent += fn(q) + '\n\n';
  });
}

function osintRunTool(toolId) {
  const q = document.getElementById('osint-query')?.value.trim() || 'fllc.net';
  const res = document.getElementById('osint-results');
  if (!res) return;
  const fn = OSINT_RESULTS[toolId];
  if (fn) res.textContent = fn(q);
}

/* ─────────────────────────────────────────────────────────────
   WORLD MAP APP
───────────────────────────────────────────────────────────── */
const MAP_ZONES = [
  { name: 'Neon City',        x: 0.25, y: 0.35, color: '#00ffcc', players: 1247, threat: 'low'  },
  { name: 'Dark Web Dungeon', x: 0.60, y: 0.65, color: '#7700ff', players:  318, threat: 'crit' },
  { name: 'SOC Command',      x: 0.45, y: 0.20, color: '#00ff44', players:  892, threat: 'med'  },
  { name: 'OSINT Hub',        x: 0.75, y: 0.30, color: '#ffdd00', players:  543, threat: 'low'  },
  { name: 'Cyber Bazaar',     x: 0.20, y: 0.70, color: '#ff6600', players:  721, threat: 'med'  },
  { name: 'Galaxy Nexus',     x: 0.80, y: 0.75, color: '#ff00ff', players:  204, threat: 'high' },
  { name: 'FLLC HQ',          x: 0.50, y: 0.50, color: '#ffffff', players: 3190, threat: 'low'  },
];

function buildMapApp(body) {
  body.innerHTML = `
    <div class="map-app">
      <canvas class="map-canvas" id="map-canvas"></canvas>
      <div class="map-legend">
        ${MAP_ZONES.map(z => `
          <div class="map-legend-item">
            <div class="legend-dot" style="background:${z.color}"></div>
            <span>${z.name} (${z.players.toLocaleString()} online)</span>
          </div>`).join('')}
      </div>
    </div>`;

  const canvas = body.querySelector('#map-canvas');
  const ctx = canvas.getContext('2d');
  let tooltip = null;
  let animFrame = 0;

  function resize() {
    canvas.width  = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
  }

  function drawMap() {
    resize();
    const W = canvas.width, H = canvas.height;
    ctx.clearRect(0, 0, W, H);

    // Background grid
    ctx.strokeStyle = 'rgba(0,255,204,0.05)';
    ctx.lineWidth = 1;
    for (let x = 0; x < W; x += 40) {
      ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, H); ctx.stroke();
    }
    for (let y = 0; y < H; y += 40) {
      ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(W, y); ctx.stroke();
    }

    // Grid lines pulsing
    animFrame++;
    const pulse = Math.sin(animFrame * 0.04) * 0.5 + 0.5;

    // Connection lines between zones
    ctx.setLineDash([4, 8]);
    MAP_ZONES.forEach((z, i) => {
      MAP_ZONES.slice(i + 1).forEach(z2 => {
        if (Math.random() > 0.6) return;
        ctx.strokeStyle = `rgba(0,255,204,${0.1 + pulse * 0.07})`;
        ctx.lineWidth = 0.5;
        ctx.beginPath();
        ctx.moveTo(z.x * W, z.y * H);
        ctx.lineTo(z2.x * W, z2.y * H);
        ctx.stroke();
      });
    });
    ctx.setLineDash([]);

    // Zone nodes
    MAP_ZONES.forEach(z => {
      const cx = z.x * W, cy = z.y * H;
      const r = 18 + pulse * 6;

      // Glow
      const grad = ctx.createRadialGradient(cx, cy, 2, cx, cy, r * 2);
      grad.addColorStop(0, z.color + 'aa');
      grad.addColorStop(1, z.color + '00');
      ctx.fillStyle = grad;
      ctx.beginPath(); ctx.arc(cx, cy, r * 2, 0, Math.PI * 2); ctx.fill();

      // Circle
      ctx.strokeStyle = z.color;
      ctx.lineWidth = 2;
      ctx.fillStyle = 'rgba(0,0,0,0.7)';
      ctx.beginPath(); ctx.arc(cx, cy, 16, 0, Math.PI * 2); ctx.fill(); ctx.stroke();

      // Label
      ctx.fillStyle = z.color;
      ctx.font = '10px "Share Tech Mono", monospace';
      ctx.textAlign = 'center';
      ctx.fillText(z.name, cx, cy + 30);
      ctx.fillStyle = '#ffffff88';
      ctx.font = '9px "Share Tech Mono", monospace';
      ctx.fillText(`${z.players} online`, cx, cy + 42);
    });

    // Tooltip
    if (tooltip) {
      const z = MAP_ZONES[tooltip];
      const cx = z.x * W, cy = z.y * H;
      ctx.fillStyle = 'rgba(10,10,30,0.95)';
      ctx.strokeStyle = z.color;
      ctx.lineWidth = 1;
      const tw = 180, th = 60;
      const tx = Math.min(W - tw - 4, cx + 20), ty = Math.max(4, cy - 30);
      ctx.beginPath(); ctx.roundRect(tx, ty, tw, th, 4); ctx.fill(); ctx.stroke();
      ctx.fillStyle = z.color;
      ctx.font = 'bold 11px "Share Tech Mono", monospace';
      ctx.textAlign = 'left';
      ctx.fillText(z.name, tx + 10, ty + 18);
      ctx.fillStyle = '#aaaacc';
      ctx.font = '10px "Share Tech Mono", monospace';
      ctx.fillText(`Players: ${z.players.toLocaleString()}`, tx + 10, ty + 34);
      ctx.fillText(`Threat: ${z.threat.toUpperCase()}`, tx + 10, ty + 50);
    }

    requestAnimationFrame(drawMap);
  }

  canvas.addEventListener('mousemove', e => {
    const rect = canvas.getBoundingClientRect();
    const mx = (e.clientX - rect.left), my = (e.clientY - rect.top);
    tooltip = null;
    MAP_ZONES.forEach((z, i) => {
      const dx = z.x * canvas.width - mx, dy = z.y * canvas.height - my;
      if (Math.sqrt(dx*dx + dy*dy) < 20) tooltip = i;
    });
  });

  canvas.addEventListener('mouseleave', () => { tooltip = null; });

  setTimeout(() => { resize(); drawMap(); }, 50);
  window.addEventListener('resize', resize);
}

/* ─────────────────────────────────────────────────────────────
   AVATAR CREATOR APP
───────────────────────────────────────────────────────────── */
const AVATAR_OPTIONS = {
  body:    ['Hacker', 'Operator', 'Ghost', 'Phantom', 'Nomad', 'Infiltrator'],
  head:    ['Hood', 'Helmet', 'Visor', 'Balaclava', 'Neural Crown', 'Void Mask'],
  armor:   ['Stealth Suit', 'Combat Rig', 'Nano Weave', 'Ghost Plate', 'SOC Vest'],
  weapon:  ['Plasma Rifle', 'Data Blade', 'EMP Cannon', 'Cyber Whip', 'None'],
  color:   ['Neon Cyan', 'Deep Violet', 'Matrix Green', 'Neon Magenta', 'Void Black'],
  class:   ['Hacker', 'Infiltrator', 'SOC Analyst', 'OSINT Recon', 'Ghost Operative'],
};

const AVATAR_COLORS = {
  'Neon Cyan': '#00ffcc', 'Deep Violet': '#7700ff',
  'Matrix Green': '#00ff44', 'Neon Magenta': '#ff00ff', 'Void Black': '#1a1a2e',
};

function buildAvatarApp(body) {
  body.innerHTML = `
    <div class="avatar-app">
      <canvas class="avatar-canvas-full" id="avatar-canvas" width="200" height="240" style="width:200px;height:240px;margin:0 auto;display:block;"></canvas>
      <div class="avatar-options-grid">
        ${Object.entries(AVATAR_OPTIONS).map(([key, opts]) => `
          <div class="avatar-option-group">
            <div class="avatar-option-label">${key.toUpperCase()}</div>
            <select class="avatar-select" id="av-${key}" onchange="renderAvatar()">
              ${opts.map(o => `<option>${o}</option>`).join('')}
            </select>
          </div>`).join('')}
      </div>
      <button class="avatar-action-btn" onclick="saveAvatar()">⚙ SAVE & DEPLOY OPERATIVE</button>
      <div id="avatar-status" style="text-align:center;font-size:11px;color:#4a6a7a;margin-top:6px;"></div>
    </div>`;

  renderAvatar();
}

function renderAvatar() {
  const canvas = document.getElementById('avatar-canvas');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  const W = canvas.width, H = canvas.height;

  const colorName = document.getElementById('av-color')?.value || 'Neon Cyan';
  const c = AVATAR_COLORS[colorName] || '#00ffcc';
  const className = document.getElementById('av-class')?.value || 'Hacker';

  ctx.clearRect(0, 0, W, H);
  ctx.fillStyle = '#0a0a1a';
  ctx.fillRect(0, 0, W, H);

  // Glow bg
  const grd = ctx.createRadialGradient(W/2, H/2, 20, W/2, H/2, 100);
  grd.addColorStop(0, c + '33');
  grd.addColorStop(1, 'transparent');
  ctx.fillStyle = grd;
  ctx.fillRect(0, 0, W, H);

  // Body silhouette
  ctx.fillStyle = c + 'cc';
  ctx.strokeStyle = c;
  ctx.lineWidth = 2;

  // Torso
  ctx.beginPath();
  ctx.roundRect(W/2 - 32, 100, 64, 80, 6);
  ctx.fill();
  ctx.stroke();

  // Head
  ctx.beginPath();
  ctx.arc(W/2, 75, 32, 0, Math.PI * 2);
  ctx.fill();
  ctx.stroke();

  // Visor / eyes
  ctx.fillStyle = '#000000aa';
  ctx.fillRect(W/2 - 18, 64, 36, 14);
  ctx.fillStyle = c;
  ctx.shadowColor = c;
  ctx.shadowBlur = 8;
  ctx.fillRect(W/2 - 14, 66, 12, 10);
  ctx.fillRect(W/2 + 2,  66, 12, 10);
  ctx.shadowBlur = 0;

  // Arms
  ctx.fillStyle = c + 'aa';
  ctx.fillRect(W/2 - 52, 105, 18, 60);
  ctx.fillRect(W/2 + 34, 105, 18, 60);

  // Legs
  ctx.fillRect(W/2 - 28, 182, 22, 50);
  ctx.fillRect(W/2 + 6,  182, 22, 50);

  // Class label
  ctx.fillStyle = c;
  ctx.font = 'bold 11px "Share Tech Mono"';
  ctx.textAlign = 'center';
  ctx.shadowColor = c;
  ctx.shadowBlur = 6;
  ctx.fillText(className.toUpperCase(), W/2, H - 8);
  ctx.shadowBlur = 0;

  // Grid overlay
  ctx.strokeStyle = c + '22';
  ctx.lineWidth = 0.5;
  for (let x = 0; x < W; x += 20) { ctx.beginPath(); ctx.moveTo(x,0); ctx.lineTo(x,H); ctx.stroke(); }
  for (let y = 0; y < H; y += 20) { ctx.beginPath(); ctx.moveTo(0,y); ctx.lineTo(W,y); ctx.stroke(); }
}

function saveAvatar() {
  const st = document.getElementById('avatar-status');
  if (st) {
    st.textContent = '✔ Operative deployed to CyberWorld!';
    st.style.color = '#00ff44';
    setTimeout(() => { st.textContent = ''; }, 3000);
  }
}

/* ─────────────────────────────────────────────────────────────
   ABOUT APP
───────────────────────────────────────────────────────────── */
function buildAboutApp(body) {
  body.innerHTML = `
    <div class="about-app">
      <pre class="about-logo">
  ██████╗██╗   ██╗██████╗ ███████╗██████╗ ██╗    ██╗ ██████╗ ██████╗ ██╗     ██████╗
 ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██║    ██║██╔═══██╗██╔══██╗██║     ██╔══██╗
 ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██║ █╗ ██║██║   ██║██████╔╝██║     ██║  ██║
 ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██║███╗██║██║   ██║██╔══██╗██║     ██║  ██║
 ╚██████╗   ██║   ██████╔╝███████╗██║  ██║╚███╔███╔╝╚██████╔╝██║  ██║███████╗██████╔╝
  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝ ╚══╝╚══╝  ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═════╝
      </pre>
      <div class="about-section">
        <div class="about-heading">◈ WHAT IS CYBERWORLD OS?</div>
        <div class="about-text">
          CyberWorld OS is a browser-based cyberpunk MMORPG operating system built by
          <strong>FLLC Enterprise IT Solutions</strong>. Inspired by TempleOS and RuneScape,
          CyberWorld combines a fully interactive OS shell with a real-time multiplayer
          cybersecurity-themed game world — running entirely in your browser.
        </div>
      </div>
      <div class="about-section">
        <div class="about-heading">⬡ CYBERWORLD MMORPG</div>
        <div class="about-text">
          Explore six unique zones — Neon City, Dark Web Dungeon, SOC Command, OSINT Hub,
          Cyber Bazaar, and Galaxy Nexus. Complete missions, craft gear, level up your operative,
          and dominate the leaderboards. All with a midnight neon galaxy aesthetic.
        </div>
      </div>
      <div class="about-section">
        <div class="about-heading">⚙ TECH STACK</div>
        <div>
          <span class="about-tag">HTML5 Canvas</span>
          <span class="about-tag">Vanilla JS</span>
          <span class="about-tag">CSS3 / Variables</span>
          <span class="about-tag">Unity WebGL</span>
          <span class="about-tag">GitHub Pages</span>
          <span class="about-tag">FLLC Enterprise</span>
        </div>
      </div>
      <div class="about-section">
        <div class="about-heading">🔗 LINKS</div>
        <div class="about-text">
          <a class="about-link" href="https://www.fllc.net" target="_blank">fllc.net</a> ·
          <a class="about-link" href="https://www.fllc.net/ops" target="_blank">Ops Center</a> ·
          <a class="about-link" href="https://www.fllc.net/cyber-arsenal" target="_blank">Cyber Arsenal</a> ·
          <a class="about-link" href="https://www.fllc.net/furios-int" target="_blank">FuriosINT</a> ·
          <a class="about-link" href="https://github.com/Personfu/CYBERWORLDSOURCECODE" target="_blank">GitHub</a>
        </div>
      </div>
      <div class="about-section">
        <div class="about-heading">© LICENSE</div>
        <div class="about-text" style="font-size:11px;color:#4a6a7a;">
          CyberWorld OS © 2026 FLLC Enterprise IT Solutions — Furulie LLC.<br/>
          All rights reserved. Unauthorized reproduction prohibited.
        </div>
      </div>
    </div>`;
}

/* ─────────────────────────────────────────────────────────────
   GAME APP — delegates to game.js
───────────────────────────────────────────────────────────── */
function buildGameApp(body) {
  body.innerHTML = `
    <div class="game-app">
      <div class="game-header">
        <div>
          <div class="game-title">⬡ CYBERWORLD MMORPG</div>
          <div class="game-subtitle">FLLC ENTERPRISE · MIDNIGHT NEON GALAXY · v1.0.0</div>
        </div>
      </div>
      <div class="game-body">
        <div class="game-sidebar">
          <div class="game-sidebar-title">ZONES</div>
          <button class="game-zone-btn active" onclick="cwZone('neon')">⬡ Neon City</button>
          <button class="game-zone-btn" onclick="cwZone('dark')">☠ Dark Web Dungeon</button>
          <button class="game-zone-btn" onclick="cwZone('soc')">◈ SOC Command</button>
          <button class="game-zone-btn" onclick="cwZone('osint')">⊕ OSINT Hub</button>
          <button class="game-zone-btn" onclick="cwZone('bazaar')">⊞ Cyber Bazaar</button>
          <button class="game-zone-btn" onclick="cwZone('galaxy')">✦ Galaxy Nexus</button>
          <div class="game-sidebar-title" style="margin-top:12px;">OPERATIVE</div>
          <div id="gm-stats" style="font-size:11px;color:#7a9aaa;line-height:1.9;">
            LVL: <span style="color:#00ffcc">1</span><br/>
            HP:  <span style="color:#00ff44">100/100</span><br/>
            XP:  <span style="color:#ffdd00">0/1000</span><br/>
            ₢:   <span style="color:#ff6600">500</span>
          </div>
          <div class="game-sidebar-title" style="margin-top:12px;">CONTROLS</div>
          <div style="font-size:10px;color:#4a6a7a;line-height:1.9;">
            WASD / ↑↓←→ Move<br/>
            E — Interact<br/>
            I — Inventory<br/>
            M — Map<br/>
            ESC — Menu
          </div>
        </div>
        <div class="game-main">
          <canvas id="game-canvas" class="game-canvas"></canvas>
          <div class="game-hud">
            <div class="hud-stat" id="hud-zone">◈ NEON CITY</div>
            <div class="hud-stat" id="hud-players">👤 1,247 ONLINE</div>
            <div class="hud-stat" id="hud-threat">⚠ THREAT: LOW</div>
          </div>
        </div>
      </div>
    </div>`;

  setTimeout(() => {
    if (typeof CyberWorldGame !== 'undefined') {
      CyberWorldGame.init('game-canvas');
    } else if (window.__cwGameScript) {
      // already loaded
    } else {
      window.__cwGameScript = true;
      const s = document.createElement('script');
      s.src = 'game.js';
      s.onload = () => { if (typeof CyberWorldGame !== 'undefined') CyberWorldGame.init('game-canvas'); };
      document.head.appendChild(s);
    }
  }, 100);
}

function cwZone(zone) {
  document.querySelectorAll('.game-zone-btn').forEach(b => b.classList.remove('active'));
  event.target.classList.add('active');
  const ZONE_INFO = {
    neon:   { label: 'NEON CITY',        players: '1,247', threat: 'LOW' },
    dark:   { label: 'DARK WEB DUNGEON', players: '318',   threat: 'CRITICAL' },
    soc:    { label: 'SOC COMMAND',      players: '892',   threat: 'MEDIUM' },
    osint:  { label: 'OSINT HUB',        players: '543',   threat: 'LOW' },
    bazaar: { label: 'CYBER BAZAAR',     players: '721',   threat: 'MEDIUM' },
    galaxy: { label: 'GALAXY NEXUS',     players: '204',   threat: 'HIGH' },
  };
  const info = ZONE_INFO[zone] || ZONE_INFO.neon;
  const hzEl = document.getElementById('hud-zone');
  const hpEl = document.getElementById('hud-players');
  const htEl = document.getElementById('hud-threat');
  if (hzEl) hzEl.textContent = `◈ ${info.label}`;
  if (hpEl) hpEl.textContent = `👤 ${info.players} ONLINE`;
  if (htEl) htEl.textContent = `⚠ THREAT: ${info.threat}`;
  if (typeof CyberWorldGame !== 'undefined') CyberWorldGame.setZone(zone);
}
