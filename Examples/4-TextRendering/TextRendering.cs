using System;
using UnityEngine;

public class TextRendering : MonoBehaviour
{

  private GiraffeLayer mLayer;
  private GiraffeSprite mWhite;

  public GiraffeFont victoriaBold;

  private const int cols = 40, rows = 20;
  public int originX, originY;
  public int sizeX, sizeY;
  public String line1 = "     **** COMMODORE 64 BASIC V2 ****";
  public String line2 = "READY.";

  public Color32 backgroundColour = new Color32(72, 58, 170, 255);
  public Color32 textColour = new Color32(134, 122, 222, 255);

  void Start()
  {
    mLayer = GetComponent<GiraffeLayer>();
    mWhite = mLayer.atlas.GetSprite("Giraffe/White");

  }


  void FixedUpdate()
  {

    // Calculate the top left coordinates.
    sizeX = victoriaBold.lineHeight * cols;
    sizeY = victoriaBold.lineHeight * rows;
    originX = (Screen.width / 2 / mLayer.scale) - sizeX / 2;
    originY = (Screen.height / 2 / mLayer.scale) - sizeY / 2;

    // Figure out how many quads the background, two lines of text and the flashing cursor will use.
    int quadLength = 0;
    quadLength += victoriaBold.Estimate(line1);
    quadLength += victoriaBold.Estimate(line2);
    quadLength += 2; // 1 for the background and 1 for the cursor.

    mLayer.Begin(quadLength + 2);

    // Draw the Background in the background colour
    mLayer.SetColour(backgroundColour);
    mLayer.Add(originX, originY, sizeX, sizeY, mWhite);

    // Draw both lines Lines of text colour
    mLayer.SetColour(textColour); // Note: We only need to change the colour when necessary, not every quad or line of text.
    victoriaBold.AddTo(mLayer, originX, originY + victoriaBold.lineHeight, line1);
    victoriaBold.AddTo(mLayer, originX, originY + victoriaBold.lineHeight * 2, line2);

    // Draw the flashing Cursor, but alternative colours every other second.
    mLayer.SetColour(((int)Time.time % 2 == 0) ? textColour : backgroundColour);
    mLayer.Add(originX, originY + victoriaBold.lineHeight * 3, victoriaBold.lineHeight, victoriaBold.lineHeight, mWhite);

    mLayer.End();
  }

}
