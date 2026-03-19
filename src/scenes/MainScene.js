export class MainScene extends Phaser.Scene {
    constructor() {
        super('MainScene');
    }

    create() {
        // Simple Grid Background
        this.add.grid(400, 300, 1600, 1200, 40, 40, 0x0a0a0a, 1, 0x0044ff, 0.2);

        // Player Icon (Placeholder as we don't have local assets yet)
        this.player = this.add.sprite(400, 300, 'player').setOrigin(0.5);
        this.player.setScale(2);

        // Movement Keys
        this.cursors = this.input.keyboard.createCursorKeys();

        // UI Information
        this.statusText = this.add.text(10, 560, 'ZONE: SECTOR_MAIN [640x480]', { 
            fontFamily: 'VT323', 
            fontSize: '16px', 
            color: '#00ff41' 
        });

        this.add.text(10, 580, 'OPERATIVE: FURIOS-INT-01', { 
            fontFamily: 'VT323', 
            fontSize: '16px', 
            color: '#00ff41' 
        });

        // Click to move (RuneScape style)
        this.input.on('pointerdown', (pointer) => {
            this.tweens.add({
                targets: this.player,
                x: Phaser.Math.Snap.To(pointer.x, 40),
                y: Phaser.Math.Snap.To(pointer.y, 40),
                duration: 500,
                ease: 'Power2'
            });
        });
    }

    update() {
        // Arrow key movement
        const speed = 4;
        if (this.cursors.left.isDown) this.player.x -= speed;
        else if (this.cursors.right.isDown) this.player.x += speed;

        if (this.cursors.up.isDown) this.player.y -= speed;
        else if (this.cursors.down.isDown) this.player.y += speed;

        // Snap to grid on release
        if (this.input.keyboard.checkDown(this.cursors.left, 500) || 
            this.input.keyboard.checkDown(this.cursors.right, 500) ||
            this.input.keyboard.checkDown(this.cursors.up, 500) ||
            this.input.keyboard.checkDown(this.cursors.down, 500)) {
            // Processing...
        }
    }
}
