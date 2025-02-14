use std::fs;
use std::io;
use std::path::Path;
use std::env;

// copy input files to output dir
fn main() -> std::io::Result<()> {
    let rust_dir: String = env::var("CARGO_MANIFEST_DIR").unwrap();

    let src_data_dir  = Path::new(&rust_dir).parent().unwrap().join(".input");
    let dest_data_dir = Path::new(&rust_dir).join("src").join(".input");

    if !fs::exists(&dest_data_dir).unwrap() {
         fs::create_dir(&dest_data_dir).unwrap();
    }
    copy_dir(src_data_dir.as_path(), dest_data_dir.as_path())?;

    println!("build::src data dir after copy:");
    print_directory_contents(src_data_dir.as_path())?;
    println!("build::target data dir after copy:");
    print_directory_contents(dest_data_dir.as_path())?;

    Ok(())
}

fn copy_dir(src_path: &Path, dest_path: &Path) -> io::Result<()> {
    for src_file in fs::read_dir(src_path)? {
        let src_file = src_file?;
        let file_name = src_file.file_name();
        let target_file = Path::new(&dest_path).join(file_name);
        if !fs::exists(&target_file).unwrap() {
            fs::copy(src_file.path(), target_file)?;
        }
    }

    Ok(())
}

fn print_directory_contents(path: &Path) -> io::Result<()> {
    if path.is_dir() {
        for entry in fs::read_dir(path)? {
            let entry = entry?;
            let path = entry.path();
            if path.is_dir() {
                println!("Directory: {}", path.display());
                print_directory_contents(&path)?;
            } else {
                println!("File: {}", path.display());
            }
        }
    }

    Ok(())
}