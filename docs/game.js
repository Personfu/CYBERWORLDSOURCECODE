/* =============================================================
   CYBERWORLD MMORPG — game.js
   Canvas tile-engine MMORPG (RuneScape-inspired, cyberpunk restyled)
   FLLC Enterprise IT Solutions // Midnight Neon Galaxy
   ============================================================= */

'use strict';

const CyberWorldGame = (() => {

  /* ── CONSTANTS ────────────────────────────────────────────── */
  const TILE  = 32;
  const COLS  = 25;
  const ROWS  = 18;
  const FPS   = 60;

  /* ── TILE TYPES ───────────────────────────────────────────── */
  const T = {
    VOID:   0,
    FLOOR:  1,
    WALL:   2,
    WATER:  3,
    ROAD:   4,
    GRASS:  5,
    PORTAL: 6,
    NPC:    7,
    CHEST:  8,
    TERM:   9,   // terminal station
    SERVER: 10,
  };

  /* ── ZONE MAPS ────────────────────────────────────────────── */
  const ZONE_DATA = {
    neon: {
      name: 'NEON CITY',
      bgColor: '#050515',
      gridColor: '#00ffcc18',
      tiles: buildNeonCity(),
      npcs: [
        { name: 'Ghost-7',   x: 5,  y: 4,  role: 'Hacker',   dialog: 'The firewall on Block 9 is hot. Careful.' },
        { name: 'Cipher-X',  x: 12, y: 8,  role: 'Trader',   dialog: 'I have exploit kits. 500 credits each.' },
        { name: 'SOC-Alpha', x: 19, y: 13, role: 'Operative', dialog: 'Three APTs active in sector 7 tonight.' },
        { name: 'Nano',      x: 7,  y: 14, role: 'Guide',     dialog: 'Welcome to Neon City. Watch the datavoids.' },
      ],
      quests: [
        { id: 'nc_001', name: 'Breach the Wall',  xp: 150, reward: 200, desc: 'Hack the firewall node on Block 9.' },
        { id: 'nc_002', name: 'Data Courier',     xp:  80, reward: 100, desc: 'Deliver encrypted payload to Cipher-X.' },
        { id: 'nc_003', name: 'Ghost Protocol',   xp: 300, reward: 500, desc: 'Go dark and infiltrate the admin zone.' },
      ],
    },
    dark: {
      name: 'DARK WEB DUNGEON',
      bgColor: '#080008',
      gridColor: '#7700ff18',
      tiles: buildDarkWeb(),
      npcs: [
        { name: 'Specter',    x: 3,  y: 6,  role: 'Vendor',   dialog: 'Zero-days. Fresh. Never been patched.' },
        { name: 'Void-Monk',  x: 16, y: 9,  role: 'Boss',     dialog: 'You dare enter my domain, script kiddie?' },
        { name: 'Nullbyte',   x: 9,  y: 15, role: 'Smuggler', dialog: 'I move data. You move fast. Deal?' },
      ],
      quests: [
        { id: 'dw_001', name: 'Darkness Protocol', xp: 400, reward: 800, desc: 'Defeat Void-Monk in the deepest node.' },
        { id: 'dw_002', name: 'Black Market Run',  xp: 200, reward: 350, desc: 'Acquire contraband data from Specter.' },
      ],
    },
    soc: {
      name: 'SOC COMMAND',
      bgColor: '#000a05',
      gridColor: '#00ff4418',
      tiles: buildSOCCommand(),
      npcs: [
        { name: 'Sentinel-1', x: 4,  y: 3,  role: 'Analyst',  dialog: 'APT-47 is probing our perimeter. Deploy countermeasures.' },
        { name: 'Commander',  x: 11, y: 8,  role: 'Commander', dialog: 'Three incidents open. Priority: ransomware on Sector 3.' },
        { name: 'Watcher',    x: 20, y: 5,  role: 'Monitor',  dialog: 'I track everything. Nothing escapes the SOC.' },
      ],
      quests: [
        { id: 'soc_001', name: 'Incident Response',  xp: 500, reward: 900, desc: 'Contain the ransomware outbreak in Sector 3.' },
        { id: 'soc_002', name: 'Threat Hunt Alpha',  xp: 350, reward: 600, desc: 'Hunt APT-47 across 12 network segments.' },
        { id: 'soc_003', name: 'Zero-Day Patch',     xp: 250, reward: 400, desc: 'Deploy emergency patches to 200 endpoints.' },
      ],
    },
    osint: {
      name: 'OSINT HUB',
      bgColor: '#0a0a00',
      gridColor: '#ffdd0018',
      tiles: buildOSINTHub(),
      npcs: [
        { name: 'Recon-9',    x: 6,  y: 5,  role: 'OSINT Analyst', dialog: 'Target is active on 3 dark web forums.' },
        { name: 'Archivist',  x: 14, y: 11, role: 'Librarian',     dialog: 'Every digital footprint tells a story.' },
        { name: 'Trace',      x: 20, y: 7,  role: 'Tracker',       dialog: 'I found 47 leaked credentials. Yours included.' },
      ],
      quests: [
        { id: 'oi_001', name: 'Digital Footprint', xp: 200, reward: 350, desc: 'Compile full OSINT profile on Target Alpha.' },
        { id: 'oi_002', name: 'Leak Hunter',       xp: 300, reward: 500, desc: 'Find 50 leaked credentials in the breach DB.' },
      ],
    },
    bazaar: {
      name: 'CYBER BAZAAR',
      bgColor: '#0a0500',
      gridColor: '#ff660018',
      tiles: buildCyberBazaar(),
      npcs: [
        { name: 'Merchant-K', x: 5,  y: 5,  role: 'Merchant',  dialog: 'Exotic gear, rare scripts, all licensed.' },
        { name: 'Forge-X',    x: 15, y: 10, role: 'Blacksmith', dialog: 'I craft the finest exploit tools.' },
        { name: 'Broker-V',   x: 10, y: 3,  role: 'Broker',    dialog: 'Information is the real currency here.' },
      ],
      quests: [
        { id: 'bz_001', name: 'Black Friday',    xp: 150, reward: 300, desc: 'Buy rare exploit from Merchant-K.' },
        { id: 'bz_002', name: 'Gear Up',         xp: 100, reward: 150, desc: 'Craft a neural implant at Forge-X.' },
      ],
    },
    galaxy: {
      name: 'GALAXY NEXUS',
      bgColor: '#000008',
      gridColor: '#ff00ff18',
      tiles: buildGalaxyNexus(),
      npcs: [
        { name: 'Starlink-0', x: 8,  y: 4,  role: 'Cosmic Hacker', dialog: 'The universe is one big network to breach.' },
        { name: 'Nebula',     x: 16, y: 12, role: 'Mystic',        dialog: 'Quantum encryption is only the beginning.' },
        { name: 'Vortex',     x: 12, y: 8,  role: 'Warlord',       dialog: 'Rule the nexus, rule the network.' },
      ],
      quests: [
        { id: 'gx_001', name: 'Quantum Breach',  xp: 800, reward: 1500, desc: 'Break quantum encryption on the Nexus Core.' },
        { id: 'gx_002', name: 'Galaxy Protocol', xp: 600, reward: 1000, desc: 'Establish a wormhole between all zones.' },
      ],
    },
  };

  /* ── MAP BUILDERS ─────────────────────────────────────────── */
  function makeMap(fn) {
    return Array.from({ length: ROWS }, (_, r) =>
      Array.from({ length: COLS }, (_, c) => fn(r, c))
    );
  }

  function buildNeonCity() {
    return makeMap((r, c) => {
      if (r === 0 || r === ROWS-1 || c === 0 || c === COLS-1) return T.WALL;
      if (r % 6 === 0) return T.ROAD;
      if (c % 8 === 0) return T.ROAD;
      if ((r === 3 && c === 10) || (r === 9 && c === 18)) return T.PORTAL;
      if ((r === 5 && c === 5)  || (r === 12 && c === 19)) return T.CHEST;
      if ((r === 4 && c === 12) || (r === 11 && c === 7))  return T.TERM;
      if (r % 3 === 1 && c % 4 === 2) return T.SERVER;
      return T.FLOOR;
    });
  }

  function buildDarkWeb() {
    return makeMap((r, c) => {
      if (r === 0 || r === ROWS-1 || c === 0 || c === COLS-1) return T.WALL;
      if ((r + c) % 7 === 0) return T.VOID;
      if (r % 5 === 2 && c % 6 !== 0) return T.WALL;
      if ((r === 8 && c === 12)) return T.PORTAL;
      if ((r === 5 && c === 18) || (r === 14 && c === 4)) return T.CHEST;
      if (r % 4 === 1 && c % 5 === 3) return T.WATER;
      return T.FLOOR;
    });
  }

  function buildSOCCommand() {
    return makeMap((r, c) => {
      if (r === 0 || r === ROWS-1 || c === 0 || c === COLS-1) return T.WALL;
      if (r === 7 || r === 12) return (c % 5 === 0) ? T.WALL : T.FLOOR;
      if (c === 5 || c === 14 || c === 22) return (r % 4 === 0) ? T.WALL : T.FLOOR;
      if ((r === 4 && c === 11) || (r === 9 && c === 20)) return T.PORTAL;
      if (r % 3 === 2 && c % 6 === 3) return T.SERVER;
      if ((r === 6 && c === 6) || (r === 13 && c === 18)) return T.TERM;
      return T.FLOOR;
    });
  }

  function buildOSINTHub() {
    return makeMap((r, c) => {
      if (r === 0 || r === ROWS-1 || c === 0 || c === COLS-1) return T.WALL;
      if (r === 8 && c > 3 && c < COLS-3) return T.WATER;
      if (r % 4 === 0 && c % 7 === 0) return T.CHEST;
      if ((r === 5 && c === 12) || (r === 13 && c === 8)) return T.PORTAL;
      if (r % 5 === 1 && c % 8 === 4) return T.TERM;
      return T.FLOOR;
    });
  }

  function buildCyberBazaar() {
    return makeMap((r, c) => {
      if (r === 0 || r === ROWS-1 || c === 0 || c === COLS-1) return T.WALL;
      if (r % 4 === 0 && c % 3 !== 0) return T.ROAD;
      if (r % 4 === 1 && c % 3 === 1) return T.CHEST;
      if ((r === 4 && c === 15) || (r === 11 && c === 7)) return T.PORTAL;
      return T.FLOOR;
    });
  }

  function buildGalaxyNexus() {
    return makeMap((r, c) => {
      if (r === 0 || r === ROWS-1 || c === 0 || c === COLS-1) return T.WALL;
      const cx = COLS/2, cy = ROWS/2;
      const dist = Math.sqrt((c-cx)**2 + (r-cy)**2);
      if (dist < 3) return T.PORTAL;
      if (Math.abs(dist - 5) < 0.6) return T.WATER;
      if (Math.abs(dist - 8) < 0.6) return T.WALL;
      if (r % 6 === 1 && c % 6 === 1) return T.CHEST;
      return T.FLOOR;
    });
  }

  /* ── TILE COLORS ──────────────────────────────────────────── */
  const TILE_STYLE = {
    [T.VOID]:   { fill: '#050510',   stroke: '#111133' },
    [T.FLOOR]:  { fill: '#0d0d22',   stroke: '#1a1a40' },
    [T.WALL]:   { fill: '#1a0a40',   stroke: '#7700ff' },
    [T.WATER]:  { fill: '#001a2a',   stroke: '#00ffcc' },
    [T.ROAD]:   { fill: '#0a1a1a',   stroke: '#00ff44' },
    [T.GRASS]:  { fill: '#0a1a0a',   stroke: '#00ff44' },
    [T.PORTAL]: { fill: '#1a003a',   stroke: '#ff00ff', glow: '#ff00ff' },
    [T.NPC]:    { fill: '#002222',   stroke: '#00ffcc' },
    [T.CHEST]:  { fill: '#1a1a00',   stroke: '#ffdd00', glow: '#ffdd00' },
    [T.TERM]:   { fill: '#002200',   stroke: '#00ff44', glow: '#00ff44' },
    [T.SERVER]: { fill: '#100010',   stroke: '#7700ff', glow: '#7700ff' },
  };

  /* ── ITEMS DATABASE ───────────────────────────────────────── */
  const ITEMS_DB = [
    { id: 'exploit_kit',   name: 'Exploit Kit v4',    type: 'weapon', atk: 15, def:  0, val: 300,  icon: '🗡️' },
    { id: 'neural_helm',   name: 'Neural Helm',        type: 'armor',  atk:  0, def: 20, val: 450,  icon: '🪖' },
    { id: 'data_blade',    name: 'Data Blade',         type: 'weapon', atk: 25, def:  5, val: 600,  icon: '🔪' },
    { id: 'ghost_suit',    name: 'Ghost Suit',         type: 'armor',  atk:  0, def: 35, val: 800,  icon: '🧥' },
    { id: 'emp_grenade',   name: 'EMP Grenade',        type: 'item',   atk: 40, def:  0, val: 200,  icon: '💣' },
    { id: 'crypto_wallet', name: 'Crypto Wallet',      type: 'item',   atk:  0, def:  0, val: 1000, icon: '💰' },
    { id: 'zero_day',      name: 'Zero-Day Exploit',   type: 'weapon', atk: 60, def:  0, val: 2000, icon: '⚡' },
    { id: 'ai_shield',     name: 'AI Defense Shield',  type: 'armor',  atk:  0, def: 50, val: 1800, icon: '🛡️' },
  ];

  /* ── GAME STATE ───────────────────────────────────────────── */
  const state = {
    zone:      'neon',
    player: {
      x: 3, y: 3,
      px: 0, py: 0,    // pixel offsets for smooth movement
      moving: false,
      dir: 'down',
      animFrame: 0,
      hp: 100, maxHp: 100,
      xp: 0, xpNext: 1000,
      lvl: 1,
      credits: 500,
      atk: 10, def: 5,
      inventory: [],
      equippedWeapon: null,
      equippedArmor: null,
      quests: [],
    },
    camera: { x: 0, y: 0 },
    tick: 0,
    dialog: null,
    combat: null,
    inventory: false,
    chat: [],
    enemies: [],
    particles: [],
    floatingTexts: [],
  };

  /* ── SPAWN ENEMIES ────────────────────────────────────────── */
  function spawnEnemies(zone) {
    const enemyTypes = {
      neon:   [{ name: 'Script Kiddie', hp: 40,  atk: 8,  def: 2,  xp: 80,  credits: 50  },
               { name: 'Firewall Bot',  hp: 60,  atk: 12, def: 5,  xp: 120, credits: 80  }],
      dark:   [{ name: 'Shadow Drone',  hp: 80,  atk: 18, def: 8,  xp: 200, credits: 150 },
               { name: 'Void Crawler',  hp: 120, atk: 22, def: 12, xp: 300, credits: 200 }],
      soc:    [{ name: 'APT Agent',     hp: 100, atk: 20, def: 10, xp: 250, credits: 180 },
               { name: 'Zero-Day Bot',  hp: 70,  atk: 30, def: 5,  xp: 200, credits: 120 }],
      osint:  [{ name: 'Data Wraith',   hp: 60,  atk: 14, def: 6,  xp: 150, credits: 100 }],
      bazaar: [{ name: 'Thief Drone',   hp: 50,  atk: 10, def: 4,  xp: 100, credits: 70  }],
      galaxy: [{ name: 'Quantum Golem', hp: 200, atk: 35, def: 20, xp: 500, credits: 400 },
               { name: 'Stellar Worm',  hp: 150, atk: 28, def: 15, xp: 400, credits: 300 }],
    };

    const types = enemyTypes[zone] || enemyTypes.neon;
    const zd = ZONE_DATA[zone];
    state.enemies = [];
    const count = 6;
    for (let i = 0; i < count; i++) {
      const t = types[Math.floor(Math.random() * types.length)];
      let ex, ey;
      do {
        ex = 2 + Math.floor(Math.random() * (COLS - 4));
        ey = 2 + Math.floor(Math.random() * (ROWS - 4));
      } while (zd.tiles[ey][ex] === T.WALL || zd.tiles[ey][ex] === T.VOID);

      state.enemies.push({
        ...t, x: ex, y: ey, maxHp: t.hp,
        dir: 'down', anim: 0,
        moveTimer: 0, moveInterval: 80 + Math.floor(Math.random() * 120),
        id: `e_${i}_${Date.now()}`,
      });
    }
  }

  /* ── CANVAS SETUP ─────────────────────────────────────────── */
  let canvas, ctx, canvasId;
  let running = false;
  let lastFrame = 0;
  let keysDown = {};

  function init(id) {
    canvasId = id;
    canvas = document.getElementById(id);
    if (!canvas) return;
    ctx = canvas.getContext('2d');
    resize();
    window.addEventListener('resize', resize);
    document.addEventListener('keydown', onKeyDown);
    document.addEventListener('keyup', e => { delete keysDown[e.key]; });
    canvas.addEventListener('click', onCanvasClick);
    loadZone('neon');
    running = true;
    requestAnimationFrame(gameLoop);
  }

  function resize() {
    if (!canvas) return;
    canvas.width  = canvas.offsetWidth  || 640;
    canvas.height = canvas.offsetHeight || 400;
  }

  /* ── INPUT ────────────────────────────────────────────────── */
  function onKeyDown(e) {
    if (!running) return;
    keysDown[e.key] = true;

    // Dialog: advance with Enter / Space
    if (state.dialog && (e.key === 'Enter' || e.key === ' ' || e.key === 'Escape')) {
      state.dialog = null;
      return;
    }
    // Combat: attack with Space/Enter
    if (state.combat && (e.key === ' ' || e.key === 'Enter')) {
      doCombatAttack();
      return;
    }
    if (state.combat && e.key === 'Escape') { state.combat = null; return; }
    // Inventory toggle
    if (e.key === 'i' || e.key === 'I') { state.inventory = !state.inventory; return; }
    // Movement
    if (!state.dialog && !state.combat) handleMovement(e.key);
  }

  function handleMovement(key) {
    const p = state.player;
    if (p.moving) return;
    let nx = p.x, ny = p.y, dir = p.dir;
    if (key === 'ArrowUp'    || key === 'w' || key === 'W') { ny--; dir = 'up'; }
    if (key === 'ArrowDown'  || key === 's' || key === 'S') { ny++; dir = 'down'; }
    if (key === 'ArrowLeft'  || key === 'a' || key === 'A') { nx--; dir = 'left'; }
    if (key === 'ArrowRight' || key === 'd' || key === 'D') { nx++; dir = 'right'; }
    if (nx === p.x && ny === p.y) return;
    tryMove(nx, ny, dir);
  }

  function tryMove(nx, ny, dir) {
    const p = state.player;
    const zd = ZONE_DATA[state.zone];
    p.dir = dir;

    if (nx < 0 || ny < 0 || nx >= COLS || ny >= ROWS) return;
    const tile = zd.tiles[ny][nx];
    if (tile === T.WALL || tile === T.VOID) return;

    // Check NPC collision → dialog
    const npc = zd.npcs.find(n => n.x === nx && n.y === ny);
    if (npc) { showDialog(npc); return; }

    // Check enemy collision → combat
    const enemy = state.enemies.find(e => e.x === nx && e.y === ny);
    if (enemy) { startCombat(enemy); return; }

    // Check chest
    if (tile === T.CHEST) {
      lootChest(nx, ny);
      zd.tiles[ny][nx] = T.FLOOR;
    }

    // Smooth move
    p.moving = true;
    p.px = p.x; p.py = p.y;
    p.x = nx; p.y = ny;

    // Portal
    if (tile === T.PORTAL) {
      const zones = Object.keys(ZONE_DATA);
      const next = zones[(zones.indexOf(state.zone) + 1) % zones.length];
      setTimeout(() => loadZone(next), 400);
    }
  }

  /* ── CANVAS CLICK → PATHFIND STEP ────────────────────────── */
  function onCanvasClick(e) {
    if (state.dialog || state.combat) return;
    const rect = canvas.getBoundingClientRect();
    const mx = e.clientX - rect.left;
    const my = e.clientY - rect.top;
    const tx = Math.floor((mx + state.camera.x) / TILE);
    const ty = Math.floor((my + state.camera.y) / TILE);
    if (tx < 0 || ty < 0 || tx >= COLS || ty >= ROWS) return;
    const p = state.player;
    const dx = Math.sign(tx - p.x);
    const dy = Math.sign(ty - p.y);
    if (dx !== 0) {
      tryMove(p.x + dx, p.y, dx > 0 ? 'right' : 'left');
    } else if (dy !== 0) {
      tryMove(p.x, p.y + dy, dy > 0 ? 'down' : 'up');
    }
  }

  /* ── DIALOG ───────────────────────────────────────────────── */
  function showDialog(npc) {
    state.dialog = {
      name: npc.name,
      role: npc.role,
      text: npc.dialog,
      quests: ZONE_DATA[state.zone].quests.filter(q => !state.player.quests.includes(q.id)),
    };
  }

  /* ── CHEST ────────────────────────────────────────────────── */
  function lootChest(x, y) {
    const item = ITEMS_DB[Math.floor(Math.random() * ITEMS_DB.length)];
    state.player.inventory.push({ ...item });
    addFloatingText(`+${item.icon} ${item.name}!`, x * TILE, y * TILE, '#ffdd00');
    addChatMsg(`📦 Found: ${item.name}`);
    updateHUD();
  }

  /* ── COMBAT ───────────────────────────────────────────────── */
  function startCombat(enemy) {
    state.combat = {
      enemy: { ...enemy },
      log: [`⚔ Combat started with ${enemy.name}!`, 'Press SPACE to attack.'],
      turn: 'player',
    };
    addChatMsg(`⚔ Engaging: ${enemy.name}`);
  }

  function doCombatAttack() {
    const cb = state.combat;
    if (!cb || cb.turn !== 'player') return;

    const p = state.player;
    const wpn = ITEMS_DB.find(i => i.id === p.equippedWeapon);
    const playerAtk = p.atk + (wpn ? wpn.atk : 0);
    const dmgToEnemy = Math.max(1, playerAtk - cb.enemy.def + Math.floor(Math.random() * 8));
    cb.enemy.hp = Math.max(0, cb.enemy.hp - dmgToEnemy);
    cb.log.push(`You deal ${dmgToEnemy} DMG to ${cb.enemy.name}.`);
    if (cb.log.length > 6) cb.log.shift();

    if (cb.enemy.hp <= 0) {
      p.xp += cb.enemy.xp;
      p.credits += cb.enemy.credits;
      addFloatingText(`+${cb.enemy.xp} XP`, p.x * TILE, p.y * TILE, '#00ff44');
      addFloatingText(`+₢${cb.enemy.credits}`, p.x * TILE, (p.y + 0.5) * TILE, '#ffdd00');
      addChatMsg(`✔ Defeated ${cb.enemy.name}! +${cb.enemy.xp} XP, +₢${cb.enemy.credits}`);
      state.enemies = state.enemies.filter(e => e.id !== cb.enemy.id);
      checkLevelUp();
      state.combat = null;
      updateHUD();
      return;
    }

    // Enemy attacks back
    cb.turn = 'enemy';
    const armor = ITEMS_DB.find(i => i.id === p.equippedArmor);
    const playerDef = p.def + (armor ? armor.def : 0);
    const dmgToPlayer = Math.max(1, cb.enemy.atk - playerDef + Math.floor(Math.random() * 6));
    p.hp = Math.max(0, p.hp - dmgToPlayer);
    cb.log.push(`${cb.enemy.name} deals ${dmgToPlayer} DMG to you.`);
    if (cb.log.length > 6) cb.log.shift();

    if (p.hp <= 0) {
      p.hp = Math.floor(p.maxHp * 0.3);
      p.credits = Math.max(0, p.credits - 50);
      addChatMsg(`💀 You were defeated! Respawning... (-₢50)`);
      state.combat = null;
      p.x = 3; p.y = 3;
    }
    cb.turn = 'player';
    updateHUD();
  }

  function checkLevelUp() {
    const p = state.player;
    while (p.xp >= p.xpNext) {
      p.xp -= p.xpNext;
      p.lvl++;
      p.xpNext = Math.floor(p.xpNext * 1.5);
      p.maxHp += 20;
      p.hp = p.maxHp;
      p.atk += 3; p.def += 2;
      addChatMsg(`⬆ LEVEL UP! You are now level ${p.lvl}!`);
      addFloatingText(`LEVEL UP! ${p.lvl}`, p.x * TILE, (p.y - 1) * TILE, '#ff00ff');
      spawnParticleBurst(p.x * TILE + TILE/2, p.y * TILE + TILE/2, '#ff00ff');
    }
  }

  /* ── FLOATING TEXT ────────────────────────────────────────── */
  function addFloatingText(text, wx, wy, color) {
    state.floatingTexts.push({ text, wx, wy, color, life: 80, alpha: 1 });
  }

  /* ── PARTICLES ────────────────────────────────────────────── */
  function spawnParticleBurst(sx, sy, color) {
    for (let i = 0; i < 24; i++) {
      const angle = (i / 24) * Math.PI * 2;
      const speed = 1 + Math.random() * 3;
      state.particles.push({
        x: sx, y: sy,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed,
        color, life: 40, alpha: 1, r: 2 + Math.random() * 2,
      });
    }
  }

  /* ── CHAT ─────────────────────────────────────────────────── */
  function addChatMsg(msg) {
    state.chat.unshift({ text: msg, time: new Date().toLocaleTimeString('en',{hour:'2-digit',minute:'2-digit',second:'2-digit'}) });
    if (state.chat.length > 12) state.chat.pop();
  }

  /* ── HUD UPDATE ───────────────────────────────────────────── */
  function updateHUD() {
    const p = state.player;
    const el = document.getElementById('gm-stats');
    if (!el) return;
    el.innerHTML = `LVL: <span style="color:#00ffcc">${p.lvl}</span><br/>
HP:  <span style="color:#${p.hp > p.maxHp*0.5 ? '00ff44' : 'ff3300'}">${p.hp}/${p.maxHp}</span><br/>
XP:  <span style="color:#ffdd00">${p.xp}/${p.xpNext}</span><br/>
₢:   <span style="color:#ff6600">${p.credits}</span><br/>
ATK: <span style="color:#ff6600">${p.atk}</span><br/>
DEF: <span style="color:#00ffcc">${p.def}</span>`;
  }

  /* ── ZONE LOAD ────────────────────────────────────────────── */
  function loadZone(zone) {
    state.zone = zone;
    state.player.x = 3;
    state.player.y = 3;
    state.dialog = null;
    state.combat = null;
    state.inventory = false;
    state.particles = [];
    state.floatingTexts = [];
    spawnEnemies(zone);
    addChatMsg(`📍 Entered: ${ZONE_DATA[zone].name}`);

    // Update HUD zone button
    document.querySelectorAll('.game-zone-btn').forEach(b => b.classList.remove('active'));
    const btn = document.querySelector(`.game-zone-btn[onclick*="'${zone}'"]`);
    if (btn) btn.classList.add('active');

    // Update HUD info
    const z = { neon:'NEON CITY', dark:'DARK WEB DUNGEON', soc:'SOC COMMAND',
                 osint:'OSINT HUB', bazaar:'CYBER BAZAAR', galaxy:'GALAXY NEXUS' };
    const p = { neon:'1,247', dark:'318', soc:'892', osint:'543', bazaar:'721', galaxy:'204' };
    const t = { neon:'LOW', dark:'CRITICAL', soc:'MEDIUM', osint:'LOW', bazaar:'MEDIUM', galaxy:'HIGH' };
    const hzEl = document.getElementById('hud-zone');
    const hpEl = document.getElementById('hud-players');
    const htEl = document.getElementById('hud-threat');
    if (hzEl) hzEl.textContent = `◈ ${z[zone] || zone}`;
    if (hpEl) hpEl.textContent = `👤 ${p[zone] || '?'} ONLINE`;
    if (htEl) htEl.textContent = `⚠ THREAT: ${t[zone] || '?'}`;

    updateHUD();
  }

  function setZone(zone) {
    if (ZONE_DATA[zone]) loadZone(zone);
  }

  /* ── ENEMY AI ─────────────────────────────────────────────── */
  function updateEnemies() {
    const zd = ZONE_DATA[state.zone];
    state.enemies.forEach(e => {
      e.moveTimer++;
      if (e.moveTimer < e.moveInterval) return;
      e.moveTimer = 0;
      const dirs = [[0,-1],[0,1],[-1,0],[1,0]];
      const d = dirs[Math.floor(Math.random() * dirs.length)];
      const nx = e.x + d[0], ny = e.y + d[1];
      if (nx < 1 || ny < 1 || nx >= COLS-1 || ny >= ROWS-1) return;
      const tile = zd.tiles[ny][nx];
      if (tile === T.WALL || tile === T.VOID) return;
      if (state.enemies.some(o => o !== e && o.x === nx && o.y === ny)) return;
      if (state.player.x === nx && state.player.y === ny) return;
      e.x = nx; e.y = ny;
      e.dir = d[0] > 0 ? 'right' : d[0] < 0 ? 'left' : d[1] > 0 ? 'down' : 'up';
    });
  }

  /* ── CAMERA ───────────────────────────────────────────────── */
  function updateCamera() {
    if (!canvas) return;
    const px = state.player.x * TILE + TILE/2;
    const py = state.player.y * TILE + TILE/2;
    const W = canvas.width, H = canvas.height;
    state.camera.x = Math.max(0, Math.min(COLS * TILE - W, px - W/2));
    state.camera.y = Math.max(0, Math.min(ROWS * TILE - H, py - H/2));
  }

  /* ── PLAYER ANIMATION ─────────────────────────────────────── */
  function updatePlayer() {
    const p = state.player;
    if (p.moving) {
      const MOVE_SPEED = 4;
      const tx = (p.x - p.px) * MOVE_SPEED;
      const ty = (p.y - p.py) * MOVE_SPEED;
      p.animX = (p.animX || 0) + tx;
      p.animY = (p.animY || 0) + ty;
      if (Math.abs(p.animX || 0) >= Math.abs((p.x - p.px) * TILE) &&
          Math.abs(p.animY || 0) >= Math.abs((p.y - p.py) * TILE)) {
        p.moving = false;
        p.animX = 0; p.animY = 0;
      }
    }
    p.animFrame = (p.animFrame + 1) % 60;
  }

  /* ── UPDATE PARTICLES ─────────────────────────────────────── */
  function updateParticles() {
    state.particles = state.particles.filter(p => {
      p.x += p.vx; p.y += p.vy;
      p.vy += 0.08;
      p.life--;
      p.alpha = p.life / 40;
      return p.life > 0;
    });
    state.floatingTexts = state.floatingTexts.filter(t => {
      t.wy -= 1.2;
      t.life--;
      t.alpha = t.life / 80;
      return t.life > 0;
    });
  }

  /* ── MAIN GAME LOOP ───────────────────────────────────────── */
  function gameLoop(now) {
    if (!running) return;
    if (now - lastFrame < 1000 / FPS) { requestAnimationFrame(gameLoop); return; }
    lastFrame = now;
    state.tick++;

    updatePlayer();
    updateCamera();
    if (state.tick % 2 === 0) updateEnemies();
    updateParticles();

    // Continuous key movement
    if (!state.dialog && !state.combat && !state.player.moving) {
      if (keysDown['ArrowUp']    || keysDown['w']) handleMovement('ArrowUp');
      if (keysDown['ArrowDown']  || keysDown['s']) handleMovement('ArrowDown');
      if (keysDown['ArrowLeft']  || keysDown['a']) handleMovement('ArrowLeft');
      if (keysDown['ArrowRight'] || keysDown['d']) handleMovement('ArrowRight');
    }

    render();
    requestAnimationFrame(gameLoop);
  }

  /* ── RENDER ───────────────────────────────────────────────── */
  function render() {
    if (!canvas || !ctx) return;
    const W = canvas.width, H = canvas.height;
    const zd = ZONE_DATA[state.zone];
    const cx = state.camera.x, cy = state.camera.y;
    const p = state.player;

    ctx.clearRect(0, 0, W, H);
    ctx.fillStyle = zd.bgColor;
    ctx.fillRect(0, 0, W, H);

    // ── TILES ──
    const startC = Math.floor(cx / TILE), endC = Math.min(COLS, startC + Math.ceil(W / TILE) + 2);
    const startR = Math.floor(cy / TILE), endR = Math.min(ROWS, startR + Math.ceil(H / TILE) + 2);

    for (let r = startR; r < endR; r++) {
      for (let c = startC; c < endC; c++) {
        const t = zd.tiles[r][c];
        const style = TILE_STYLE[t] || TILE_STYLE[T.FLOOR];
        const sx = c * TILE - cx;
        const sy = r * TILE - cy;

        if (style.glow) {
          const pulse = Math.sin(state.tick * 0.07 + c + r) * 0.3 + 0.7;
          ctx.shadowColor = style.glow;
          ctx.shadowBlur = 8 * pulse;
        } else {
          ctx.shadowBlur = 0;
        }

        ctx.fillStyle = style.fill;
        ctx.fillRect(sx, sy, TILE, TILE);
        ctx.strokeStyle = style.stroke;
        ctx.lineWidth = 0.5;
        ctx.strokeRect(sx + 0.5, sy + 0.5, TILE - 1, TILE - 1);

        // Tile icons
        if (t === T.PORTAL) {
          ctx.font = `${TILE - 8}px serif`;
          ctx.textAlign = 'center';
          ctx.fillStyle = '#ff00ff';
          ctx.fillText('⬡', sx + TILE/2, sy + TILE - 6);
        } else if (t === T.CHEST) {
          ctx.font = `${TILE - 10}px serif`;
          ctx.textAlign = 'center';
          ctx.fillStyle = '#ffdd00';
          ctx.fillText('📦', sx + TILE/2, sy + TILE - 4);
        } else if (t === T.TERM) {
          ctx.font = `${TILE - 10}px serif`;
          ctx.textAlign = 'center';
          ctx.fillStyle = '#00ff44';
          ctx.fillText('💻', sx + TILE/2, sy + TILE - 4);
        } else if (t === T.SERVER) {
          ctx.font = `${TILE - 12}px serif`;
          ctx.textAlign = 'center';
          ctx.fillStyle = '#7700ff';
          ctx.fillText('🖥', sx + TILE/2, sy + TILE - 4);
        }
      }
    }

    ctx.shadowBlur = 0;

    // ── NPC SPRITES ──
    zd.npcs.forEach(npc => {
      const sx = npc.x * TILE - cx;
      const sy = npc.y * TILE - cy;
      if (sx < -TILE || sy < -TILE || sx > W + TILE || sy > H + TILE) return;
      drawCharacter(ctx, sx, sy, '#00ffcc', npc.name, true);
    });

    // ── ENEMIES ──
    state.enemies.forEach(e => {
      const sx = e.x * TILE - cx;
      const sy = e.y * TILE - cy;
      if (sx < -TILE || sy < -TILE || sx > W + TILE || sy > H + TILE) return;
      drawCharacter(ctx, sx, sy, '#ff003c', e.name, false);
      // HP bar
      const hpPct = e.hp / e.maxHp;
      ctx.fillStyle = '#330000';
      ctx.fillRect(sx + 2, sy - 6, TILE - 4, 4);
      ctx.fillStyle = hpPct > 0.5 ? '#00ff44' : hpPct > 0.25 ? '#ff6600' : '#ff003c';
      ctx.fillRect(sx + 2, sy - 6, (TILE - 4) * hpPct, 4);
    });

    // ── PLAYER ──
    const playerDrawX = p.x * TILE - cx + (p.animX || 0) / (p.moving ? 1 : 1);
    const playerDrawY = p.y * TILE - cy + (p.animY || 0) / (p.moving ? 1 : 1);
    drawPlayer(ctx, playerDrawX, playerDrawY);

    // ── PARTICLES ──
    state.particles.forEach(pt => {
      ctx.globalAlpha = pt.alpha;
      ctx.fillStyle = pt.color;
      ctx.shadowColor = pt.color;
      ctx.shadowBlur = 4;
      ctx.beginPath();
      ctx.arc(pt.x - cx, pt.y - cy, pt.r, 0, Math.PI * 2);
      ctx.fill();
    });
    ctx.globalAlpha = 1;
    ctx.shadowBlur = 0;

    // ── FLOATING TEXT ──
    state.floatingTexts.forEach(ft => {
      ctx.globalAlpha = ft.alpha;
      ctx.fillStyle = ft.color;
      ctx.shadowColor = ft.color;
      ctx.shadowBlur = 6;
      ctx.font = 'bold 12px "Share Tech Mono", monospace';
      ctx.textAlign = 'center';
      ctx.fillText(ft.text, ft.wx - cx, ft.wy - cy);
    });
    ctx.globalAlpha = 1;
    ctx.shadowBlur = 0;

    // ── DIALOG BOX ──
    if (state.dialog) renderDialog();

    // ── COMBAT BOX ──
    if (state.combat) renderCombat();

    // ── INVENTORY ──
    if (state.inventory) renderInventory();

    // ── MINI-MAP ──
    renderMiniMap(W, H);

    // ── CHAT LOG ──
    renderChat(W, H);
  }

  /* ── DRAW CHARACTER ───────────────────────────────────────── */
  function drawCharacter(ctx, sx, sy, color, name, isNPC) {
    const bob = Math.sin(state.tick * 0.12) * 2;

    // Body
    ctx.fillStyle = color + '88';
    ctx.strokeStyle = color;
    ctx.lineWidth = 1.5;
    ctx.shadowColor = color;
    ctx.shadowBlur = 6;

    // Torso
    ctx.beginPath();
    ctx.roundRect(sx + 8, sy + 14 + bob, 16, 12, 2);
    ctx.fill(); ctx.stroke();

    // Head
    ctx.beginPath();
    ctx.arc(sx + TILE/2, sy + 11 + bob, 7, 0, Math.PI * 2);
    ctx.fill(); ctx.stroke();

    // Eyes
    ctx.fillStyle = '#000';
    ctx.fillRect(sx + 11, sy + 8 + bob, 3, 3);
    ctx.fillRect(sx + 18, sy + 8 + bob, 3, 3);
    ctx.fillStyle = color;
    ctx.shadowBlur = 3;
    ctx.fillRect(sx + 12, sy + 9 + bob, 2, 2);
    ctx.fillRect(sx + 19, sy + 9 + bob, 2, 2);

    ctx.shadowBlur = 0;

    // Name label
    ctx.fillStyle = color;
    ctx.font = '9px "Share Tech Mono", monospace';
    ctx.textAlign = 'center';
    ctx.fillText(name.substring(0, 10), sx + TILE/2, sy + 4);
  }

  function drawPlayer(ctx, sx, sy) {
    const p = state.player;
    const bob = Math.sin(state.tick * 0.15) * 2;
    const color = '#00ffcc';

    // Shadow glow
    ctx.shadowColor = color;
    ctx.shadowBlur = 12;

    // Body
    ctx.fillStyle = color + 'aa';
    ctx.strokeStyle = color;
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.roundRect(sx + 7, sy + 13 + bob, 18, 14, 3);
    ctx.fill(); ctx.stroke();

    // Head
    ctx.beginPath();
    ctx.arc(sx + TILE/2, sy + 10 + bob, 8, 0, Math.PI * 2);
    ctx.fill(); ctx.stroke();

    // Visor
    ctx.fillStyle = '#000b';
    ctx.fillRect(sx + 9, sy + 6 + bob, 14, 6);
    ctx.fillStyle = color;
    ctx.shadowBlur = 5;
    ctx.fillRect(sx + 10, sy + 7 + bob, 5, 4);
    ctx.fillRect(sx + 17, sy + 7 + bob, 5, 4);

    ctx.shadowBlur = 0;

    // OPERATIVE label
    ctx.fillStyle = color;
    ctx.font = '8px "Share Tech Mono", monospace';
    ctx.textAlign = 'center';
    ctx.fillText(`LVL ${p.lvl}`, sx + TILE/2, sy + 3);
  }

  /* ── DIALOG RENDER ────────────────────────────────────────── */
  function renderDialog() {
    const d = state.dialog;
    const W = canvas.width, H = canvas.height;
    const bw = Math.min(520, W - 40), bh = 140;
    const bx = (W - bw) / 2, by = H - bh - 20;

    ctx.fillStyle = 'rgba(5,5,20,0.95)';
    ctx.strokeStyle = '#00ffcc';
    ctx.lineWidth = 1.5;
    ctx.shadowColor = '#00ffcc';
    ctx.shadowBlur = 10;
    ctx.beginPath();
    ctx.roundRect(bx, by, bw, bh, 6);
    ctx.fill(); ctx.stroke();
    ctx.shadowBlur = 0;

    ctx.fillStyle = '#00ffcc';
    ctx.font = 'bold 12px "Share Tech Mono", monospace';
    ctx.textAlign = 'left';
    ctx.fillText(`◈ ${d.name}  [${d.role}]`, bx + 14, by + 22);

    ctx.fillStyle = '#aabbcc';
    ctx.font = '12px "Share Tech Mono", monospace';
    wrapText(ctx, d.text, bx + 14, by + 44, bw - 28, 18);

    if (d.quests && d.quests.length > 0) {
      ctx.fillStyle = '#ffdd00';
      ctx.font = '10px "Share Tech Mono", monospace';
      ctx.fillText(`★ Available: ${d.quests[0].name} (+${d.quests[0].xp} XP, +₢${d.quests[0].reward})`, bx + 14, by + 100);
    }

    ctx.fillStyle = '#4a6a7a';
    ctx.font = '10px "Share Tech Mono", monospace';
    ctx.textAlign = 'right';
    ctx.fillText('[ENTER / SPACE to close]', bx + bw - 10, by + bh - 10);
  }

  /* ── COMBAT RENDER ────────────────────────────────────────── */
  function renderCombat() {
    const cb = state.combat;
    const W = canvas.width, H = canvas.height;
    const bw = Math.min(480, W - 40), bh = 200;
    const bx = (W - bw) / 2, by = H - bh - 20;

    ctx.fillStyle = 'rgba(20,0,5,0.97)';
    ctx.strokeStyle = '#ff003c';
    ctx.lineWidth = 2;
    ctx.shadowColor = '#ff003c';
    ctx.shadowBlur = 12;
    ctx.beginPath();
    ctx.roundRect(bx, by, bw, bh, 6);
    ctx.fill(); ctx.stroke();
    ctx.shadowBlur = 0;

    ctx.fillStyle = '#ff003c';
    ctx.font = 'bold 13px "Share Tech Mono", monospace';
    ctx.textAlign = 'left';
    ctx.fillText(`⚔ COMBAT: ${cb.enemy.name}`, bx + 14, by + 22);

    // Enemy HP bar
    const hpPct = cb.enemy.hp / cb.enemy.maxHp;
    ctx.fillStyle = '#330000';
    ctx.fillRect(bx + 14, by + 30, bw - 28, 10);
    ctx.fillStyle = hpPct > 0.5 ? '#00ff44' : hpPct > 0.25 ? '#ff6600' : '#ff003c';
    ctx.fillRect(bx + 14, by + 30, (bw - 28) * hpPct, 10);
    ctx.fillStyle = '#aabbcc';
    ctx.font = '10px "Share Tech Mono", monospace';
    ctx.fillText(`${cb.enemy.hp}/${cb.enemy.maxHp} HP`, bx + 14, by + 56);

    // Combat log
    cb.log.slice(-4).forEach((line, i) => {
      ctx.fillStyle = line.startsWith('You') ? '#00ffcc' : line.startsWith('⚔') ? '#ffdd00' : '#ff6600';
      ctx.font = '11px "Share Tech Mono", monospace';
      ctx.fillText(line, bx + 14, by + 80 + i * 18);
    });

    ctx.fillStyle = '#4a6a7a';
    ctx.font = '10px "Share Tech Mono", monospace';
    ctx.textAlign = 'right';
    ctx.fillText('[SPACE — Attack | ESC — Flee]', bx + bw - 10, by + bh - 10);
  }

  /* ── INVENTORY RENDER ─────────────────────────────────────── */
  function renderInventory() {
    const W = canvas.width, H = canvas.height;
    const bw = Math.min(400, W - 40), bh = Math.min(320, H - 60);
    const bx = W - bw - 20, by = 40;

    ctx.fillStyle = 'rgba(5,5,20,0.97)';
    ctx.strokeStyle = '#7700ff';
    ctx.lineWidth = 1.5;
    ctx.shadowColor = '#7700ff';
    ctx.shadowBlur = 10;
    ctx.beginPath();
    ctx.roundRect(bx, by, bw, bh, 6);
    ctx.fill(); ctx.stroke();
    ctx.shadowBlur = 0;

    ctx.fillStyle = '#7700ff';
    ctx.font = 'bold 13px "Share Tech Mono", monospace';
    ctx.textAlign = 'left';
    ctx.fillText('⚙ INVENTORY  [I to close]', bx + 14, by + 22);

    if (state.player.inventory.length === 0) {
      ctx.fillStyle = '#4a6a7a';
      ctx.font = '12px "Share Tech Mono", monospace';
      ctx.fillText('Empty — loot some chests!', bx + 14, by + 60);
      return;
    }

    state.player.inventory.slice(0, 8).forEach((item, i) => {
      const ix = bx + 14, iy = by + 44 + i * 32;
      ctx.fillStyle = i % 2 === 0 ? 'rgba(0,0,40,0.4)' : 'transparent';
      ctx.fillRect(bx + 4, iy - 14, bw - 8, 30);
      ctx.fillStyle = '#aabbcc';
      ctx.font = '11px "Share Tech Mono", monospace';
      ctx.fillText(`${item.icon} ${item.name}  [${item.type}]  ATK:${item.atk}  DEF:${item.def}  ₢${item.val}`, ix, iy);
    });
  }

  /* ── MINI MAP ─────────────────────────────────────────────── */
  function renderMiniMap(W, H) {
    const mw = 120, mh = 80;
    const mx = W - mw - 8, my = 8;
    const zd = ZONE_DATA[state.zone];

    ctx.fillStyle = 'rgba(5,5,20,0.85)';
    ctx.strokeStyle = '#00ffcc44';
    ctx.lineWidth = 1;
    ctx.fillRect(mx, my, mw, mh);
    ctx.strokeRect(mx, my, mw, mh);

    const tw = mw / COLS, th = mh / ROWS;
    for (let r = 0; r < ROWS; r++) {
      for (let c = 0; c < COLS; c++) {
        const t = zd.tiles[r][c];
        const style = TILE_STYLE[t];
        ctx.fillStyle = t === T.WALL  ? '#440066aa' :
                        t === T.WATER ? '#003322aa' :
                        t === T.ROAD  ? '#003311aa' :
                        t === T.PORTAL? '#ff00ff88' :
                        t === T.CHEST ? '#ffdd0088' :
                        t === T.VOID  ? '#000000'   : '#0d0d2288';
        ctx.fillRect(mx + c * tw, my + r * th, tw, th);
      }
    }

    // Enemies
    state.enemies.forEach(e => {
      ctx.fillStyle = '#ff003c';
      ctx.fillRect(mx + e.x * tw - 1, my + e.y * th - 1, 3, 3);
    });

    // Player
    const p = state.player;
    ctx.fillStyle = '#00ffcc';
    ctx.shadowColor = '#00ffcc';
    ctx.shadowBlur = 4;
    ctx.fillRect(mx + p.x * tw - 2, my + p.y * th - 2, 4, 4);
    ctx.shadowBlur = 0;

    // Zone label
    ctx.fillStyle = '#00ffcc88';
    ctx.font = '8px "Share Tech Mono", monospace';
    ctx.textAlign = 'center';
    ctx.fillText(ZONE_DATA[state.zone].name, mx + mw/2, my + mh + 11);
  }

  /* ── CHAT LOG ─────────────────────────────────────────────── */
  function renderChat(W, H) {
    if (state.chat.length === 0) return;
    const lineH = 16, pad = 6;
    const count = Math.min(state.chat.length, 5);
    const bh = count * lineH + pad * 2;
    const bw = Math.min(360, W * 0.4);
    const bx = 8, by = H - bh - 8;

    ctx.fillStyle = 'rgba(5,5,20,0.75)';
    ctx.fillRect(bx, by, bw, bh);

    state.chat.slice(0, count).reverse().forEach((msg, i) => {
      ctx.fillStyle = '#7a9aaa';
      ctx.font = '9px "Share Tech Mono", monospace';
      ctx.textAlign = 'left';
      ctx.fillText(msg.time, bx + pad, by + pad + i * lineH + 10);
      ctx.fillStyle = '#aabbcc';
      ctx.font = '10px "Share Tech Mono", monospace';
      ctx.fillText(msg.text.substring(0, 40), bx + pad + 52, by + pad + i * lineH + 10);
    });
  }

  /* ── TEXT WRAP ────────────────────────────────────────────── */
  function wrapText(ctx, text, x, y, maxW, lineH) {
    const words = text.split(' ');
    let line = '';
    let cy = y;
    words.forEach(word => {
      const test = line + word + ' ';
      if (ctx.measureText(test).width > maxW && line !== '') {
        ctx.fillText(line, x, cy);
        line = word + ' ';
        cy += lineH;
      } else {
        line = test;
      }
    });
    ctx.fillText(line, x, cy);
  }

  /* ── PUBLIC API ───────────────────────────────────────────── */
  return { init, setZone };

})();
