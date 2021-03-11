<?php  

$scanned_files = array();

function nav($dir){

    global $scanned_files;
    $main = array_diff(scandir($dir), array('..', '.'));

    if (!count($main))
        return;

    foreach($main as $sub) {

        if(is_dir($dir . '/' . $sub)) {

        	nav($dir . '/' . $sub);
        }
        else {

        	$scanned = array("NAME" => $sub, "DIRECTORY" => $dir, "SIZE" => filesize($dir . '/' . $sub), "HASH" => hash_file("md5", $dir . '/' . $sub), "ADDRESS" => "http://" . $_SERVER['SERVER_NAME'] . '/' . $dir . '/' . $sub);
		$scanned_files["FILES"][] = $scanned;
        }
    }
}
nav('Preference files');

$json = fopen('PreferenceFiles.json', 'w');

fwrite($json, json_encode($scanned_files));
fclose($json);

echo "done";

?>
