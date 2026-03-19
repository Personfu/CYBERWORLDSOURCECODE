export class BootScene extends Phaser.Scene {
    constructor() {
        super('BootScene');
    }

    preload() {
        // Load basic assets
        this.load.image('player', 'https://win98icons.alexmeub.com/icons/png/address_book_users.png');
        this.load.image('tile', 'https://win98icons.alexmeub.com/icons/png/world-0.png');
    }

    create() {
        this.add.text(400, 300, 'HOLY_BOOT(MMO_KERNEL)...', { 
            fontFamily: 'VT323', 
            fontSize: '32px', 
            color: '#00e8ff' 
        }).setOrigin(0.5);

        this.add.text(400, 340, 'Compiling HolyC grid data...', { 
            fontFamily: 'VT323', 
            fontSize: '18px', 
            color: '#00aa00' 
        }).setOrigin(0.5);

        setTimeout(() => {
            this.scene.start('MainScene');
        }, 2000);
    }
}
