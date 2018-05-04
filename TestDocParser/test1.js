/**
 * @file
 * The tile feature set.
 */

/**
 * A tile is an app's representation on the Start menu, every app has a tile. Windows displays this tile when your app is first installed.
 * After your app is installed, you can change your tile's content through notifications.
 * With this snippet you can personalize your tile experience.
 *
 * @alias Create Tile
 * @method createTile
 * @param {string} text Text to display on the tile.
 * @param {float} durationSeconds Duration to display the tile, in seconds. Defaults to 10.
 *
 */

function createTile(text, durationSeconds = 10) {
    var notifications = Windows.UI.Notifications;
    var template = notifications.TileTemplateType.tileWideImageAndText01;
    var tileXml = notifications.TileUpdateManager.getTemplateContent(template);

    var tileTextAttributes = tileXml.getElementsByTagName("text");
    tileTextAttributes[0].appendChild(tileXml.createTextNode(text));

    var tileImage = tileXml.getElementsByTagName("image");

    tileImage[0].attributes['src'] = tileImage;

    var tileNotification = new notifications.TileNotification(tileXml);
    var currentTime = new Date();
    tileNotification.expirationTime = new Date(currentTime.getTime() + durationSeconds * 1000);
    notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);
}

document.addEventListener("DOMContentLoaded", createTile, false);

/** This is a short description of the nothing function. */
function nothing() {

}

/**
 * Secondary tiles allow users to pin specific content and deep links from your app onto their Start menu, providing easy future access to the content within your app. 
 * This snippet allow you to creates, enumerates, and provides information about a secondary tile.
 *
 * @alias Create Secondary Tile
 * @method createSecondaryTile
 * @param {string} text Text to display on the secondary tile.
 * @param {string} activationArguments Arguments to include when the tile activates the app.
 * @param {string} tileId Id of the secondary tile (so it can be replaced by a matching id). Defaults to the activationArguments.
 * @param {string} logoUri Uri of the logo to display on the tile.
 * @param {string} uriSmallLogo Uri of the small logo to display on the tile.
 * @see https://raw.githubusercontent.com/JimGaleForce/Windows-universal-js-samples/master/win10/images/pinCommand.PNG
 * @returns {Promise} promise.
 */

function createSecondaryTile(text, activationArguments, tileId = null, logoUri = null, uriSmallLogo = null) {
    var currentTime = new Date();
    logoUri = logoUri || new Windows.Foundation.Uri("ms-appx:///images/Square150x150Logo.png");
    uriSmallLogo = uriSmallLogo || new Windows.Foundation.Uri("ms-appx:///images/Square44x44Logo.png");
    var newTileDesiredSize = Windows.UI.StartScreen.TileOptions.showNameOnLogo;
    tileId = tileId || arctivationArguments;

    var tile;
    try {
        tile = new Windows.UI.StartScreen.SecondaryTile(tileId, text, text, activationArguments,
            newTileDesiredSize, logoUri);
    } catch (e) {
        //Utils.error('failed to create secondary tile', e);
        return;
    }
    var element = document.body;
    if (element) {
        var selectionRect = element.getBoundingClientRect();
        var buttonCoordinates = { x: selectionRect.left, y: selectionRect.top, width: selectionRect.width, height: selectionRect.height };
        var placement = Windows.UI.Popups.Placement.above;
        return new Promise((resolve, reject) => {
            try {
                tile.requestCreateForSelectionAsync(buttonCoordinates, placement).done((isCreated) => {
                    if (isCreated) {
                        resolve(true);
                    } else {
                        reject(false);
                    }
                });
            } catch (e) {
                //Utils.error('failed to create secondary tile', e);
                reject(false);
            }
        });
    } else {
        //Utils.debug('No element to place (shall I pin a tile) question above:' + elementId);
        return new Promise(async (resolve, reject) => {
            reject(false);
        });
    }
}

document.addEventListener("DOMContentLoaded", createSecondaryTile, false);