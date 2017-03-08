class Exit extends Teleporter
{
  Exit()
  {
    size = blockSize;
    image = loadImage("exit.png");
    image_2 = loadImage("exit2.png");
    image.resize(size, size);
    image_2.resize(size, size);
  }
  
  void teleportPlayer()
  {
    newGame();
    state = GameState.PLAY;
  }
}