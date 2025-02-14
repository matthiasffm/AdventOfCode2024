// copy input files to output dir
fn main() -> std::io::Result<()> {
    use std::fs;
    use std::path::Path;
    use std::env;
    
    let rust_dir: String = env::var("CARGO_MANIFEST_DIR").unwrap();

    let src_data_dir  = Path::new(&rust_dir).parent().unwrap().join(".input");
    let dest_data_dir = Path::new(&rust_dir).join("src/.input");

    if !fs::exists(&dest_data_dir).unwrap() {
         fs::create_dir(&dest_data_dir).unwrap();
    }
    let src_file    = Path::new(&src_data_dir).join("day01.data");
    let target_file = Path::new(&dest_data_dir).join("day01.data");
    if !fs::exists(&target_file).unwrap() {
         fs::copy(&src_file, &target_file)?;
    }

    Ok(())
}