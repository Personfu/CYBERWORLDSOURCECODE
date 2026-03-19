/**
 * CYBERWORLD UNIFIED GAME ENTRY POINT
 * ═══════════════════════════════════
 * 
 * This file has been UNIFIED — all source codes compiled into ONE.
 * 
 * CyberWorld = ClubPenguin + RuneScape + Pokémon + Hacking + Cybersecurity + FLLC
 * 
 * Sub-Engines:
 *   ENGINE 1: CORE MMO (CyberWorld) - Avatar system, multiplayer
 *   ENGINE 2: SOURCECODE OPS (CYBERWORLDSOURCECODE) - Adversary combat, tools
 *   ENGINE 3: LITE OSINT (CYBERWORLDSOURCECODELITE) - Grid exploration, recon
 *   ENGINE 4: SOULCODE (CYBERWORLDSOULCODE) - Daemon capture, evolution
 * 
 * Built by Preston Furulie — FLLC // CyberOS v2026.3
 */

import { SceneBoot } from './scenes/SceneBoot.js';
import { SceneLogin } from './scenes/SceneLogin.js';
import { SceneCharacter } from './scenes/SceneCharacter.js';
import { SceneWorld } from './scenes/SceneWorld.js';
import { SceneHUD } from './scenes/SceneHUD.js';

const config = {
    type: Phaser.AUTO,
    width: 1024,
    height: 768,
    parent: 'game-container',
    backgroundColor: '#050a0f',
    pixelArt: true,
    physics: {
        default: 'arcade',
        arcade: {
            gravity: { y: 0 },
            debug: false
        }
    },
    scene: [SceneBoot, SceneLogin, SceneCharacter, SceneWorld, SceneHUD]
};

console.log(`
╔══════════════════════════════════════════════╗
║  CYBERWORLD UNIFIED ENGINE v2026.3-FLLC     ║
║  All source codes compiled into ONE         ║
║  ClubPenguin + RuneScape + Pokémon + Hack   ║
║  Built by Preston Furulie // FLLC           ║
╚══════════════════════════════════════════════╝
`);

const game = new Phaser.Game(config);
export default game;
