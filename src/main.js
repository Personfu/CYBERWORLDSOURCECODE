import { BootScene } from './scenes/BootScene.js';
import { MainScene } from './scenes/MainScene.js';

const config = {
    type: Phaser.AUTO,
    width: 800,
    height: 600,
    parent: 'game-container',
    backgroundColor: '#0a0a0a',
    pixelArt: true,
    physics: {
        default: 'arcade',
        arcade: {
            gravity: { y: 0 },
            debug: false
        }
    },
    scene: [BootScene, MainScene]
};

const game = new Phaser.Game(config);
export default game;
