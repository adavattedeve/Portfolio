package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.GlyphLayout;

/**
 * Created by atter on 31-Jan-17.
 */

public class TextObject {
    private State state;
    private BitmapFont font;
    private float x, y;
    private GlyphLayout layout;

    public TextObject(BitmapFont font, State state) {
        this("", 0, 0, font, state);
    }

    public TextObject(String text, float x, float y, BitmapFont font, State state) {
        this.state = state;
        this.font = font;
        this.x = x;
        this.y = y;
        layout = new GlyphLayout();
        layout.setText(font, text);
        state.addTextObject(this);
    }

    public BitmapFont getFont() {
        return font;
    }

    public GlyphLayout getLayout() {
        return layout;
    }

    public float getX() {
        return x;
    }

    public float getY() {
        return y;
    }

    public float getWidth() {
        return layout.width / state.getGuiCamToCamW();
    }

    public float getHeight() {
        return layout.height / state.getGuiCamToCamH();
    }

    public void setText(String newText) {
        layout.setText(font, newText);
    }
    public void setPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }
    public void dispose() {
        state.removeTextObject(this);
    }
}
