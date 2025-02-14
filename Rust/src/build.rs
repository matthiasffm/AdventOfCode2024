use std::fs;
use std::io;
use std::path::Path;
use std::env;

// copy data files from .input to src dir for build and unit tests
fn main() -> std::io::Result<()> {
    let rust_dir: String = env::var("CARGO_MANIFEST_DIR").unwrap();

    let src_data_dir  = Path::new(&rust_dir).parent().unwrap().join(".input");
    let dest_data_dir = Path::new(&rust_dir).join("src").join(".input");

    if !fs::exists(&dest_data_dir).unwrap() {
         fs::create_dir(&dest_data_dir).unwrap();
    }
    copy_dir(src_data_dir.as_path(), dest_data_dir.as_path())?;

    Ok(())
}

fn copy_dir(src_path: &Path, dest_path: &Path) -> io::Result<()> {
    for src_file in fs::read_dir(src_path)? {
        let src_file    = src_file?;
        let file_name   = src_file.file_name();
        let target_file = Path::new(&dest_path).join(file_name);

        if !fs::exists(&target_file).unwrap() {
            fs::copy(src_file.path(), target_file)?;
        }
    }

    Ok(())
}
