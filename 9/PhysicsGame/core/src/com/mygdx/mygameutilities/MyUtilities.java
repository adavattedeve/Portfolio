package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.g2d.TextureRegion;

/**
 * Created by atter on 09-Feb-17.
 */

public class MyUtilities {

    public static TextureRegion[] transformTo1D(TextureRegion[][] sheet,
                                                           int colums, int rows, int frames) {
        TextureRegion[] array = new TextureRegion[frames];

        int index = 0;

        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < colums; ++x) {

                if (index == frames) {
                    return array;
                }
                array[index] = sheet[y][x];
                ++index;
            }
        }

        return array;
    }
}
