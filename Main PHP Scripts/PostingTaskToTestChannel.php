<?php
require __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

try {
    // We'll read from POST for this example (could also read from GET or another method).
    // 1) "content" => the text to post (or edit).
    // 2) "discordID" => existing Discord post ID (if editing or deleting).
    // 3) "delete" => "true" if deleting.
    
    // Grab input parameters
    $content   = $_POST['content']   ?? '';
    $discordID = $_POST['discordID'] ?? '';
    $delete    = (isset($_POST['delete']) && $_POST['delete'] === 'true');
    
    // Use your $disWHTestFlights webhook
    global $disWHTestFlights;
    
    // SCENARIO #3: Deleting an existing post
    if ($delete) {
        if (empty($discordID)) {
            throw new Exception("Discord post ID is required to delete a post.");
        }
        
        // Call manageDiscordPost to delete
        $deleteResult = manageDiscordPost($disWHTestFlights, '', $discordID, true);
        $deleteObj = json_decode($deleteResult, true);
        
        if ($deleteObj['result'] === "success") {
            echo json_encode([
                'status'  => 'success',
                'message' => 'Post deleted successfully.'
            ]);
        } else {
            echo json_encode([
                'status'  => 'error',
                'message' => $deleteObj['error'] ?? 'Unknown error deleting post.'
            ]);
        }
        exit; // We're done once we delete
    }
    
    // SCENARIO #1 or #2: Creating or Editing
    if (empty($discordID)) {
        // No discordID => Creating a new post
        if (empty($content)) {
            throw new Exception("Content is required to create a new post.");
        }
        
        // Call manageDiscordPost to create
        $createResult = manageDiscordPost($disWHTestFlights, $content, null, false);
        $createObj = json_decode($createResult, true);
        
        if ($createObj['result'] === "success") {
            echo json_encode([
                'status'  => 'success',
                'message' => 'Post created successfully.',
                'postID'  => $createObj['postID']
            ]);
        } else {
            echo json_encode([
                'status'  => 'error',
                'message' => $createObj['error'] ?? 'Unknown error creating post.'
            ]);
        }
    } else {
        // SCENARIO #2: Editing an existing post
        if (empty($content)) {
            throw new Exception("Content is required to edit an existing post.");
        }
        
        // Call manageDiscordPost to edit
        $editResult = manageDiscordPost($disWHTestFlights, $content, $discordID, false);
        $editObj = json_decode($editResult, true);
        
        if ($editObj['result'] === "success") {
            echo json_encode([
                'status'  => 'success',
                'message' => 'Post updated successfully.',
                'postID'  => $editObj['postID']
            ]);
        } else {
            echo json_encode([
                'status'  => 'error',
                'message' => $editObj['error'] ?? 'Unknown error editing post.'
            ]);
        }
    }
    
} catch (Exception $e) {
    // If any exception is thrown, return an error response
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
}
?>
