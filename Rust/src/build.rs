// copy input files to output dir
fn main() -> std::io::Result<()> {
    use std::fs;
    use std::env;
    
    let src_dir    = env::var("CARGO_MANIFEST_DIR").unwrap() + "\\..\\.input";
    let target_dir = env::var("OUT_DIR").unwrap() + "\\..\\..\\..\\deps\\.input";
    println!("sourcedir = {}", &src_dir);
    println!("targetDir = {}", &target_dir);

    if !fs::exists(&target_dir).unwrap() {
        fs::create_dir(&target_dir).unwrap();
    }
    let target_file = target_dir + "\\day01.data";
    if !fs::exists(&target_file).unwrap() {
        fs::copy(src_dir + "\\day01.data", &target_file)?;
    }

    Ok(())
}