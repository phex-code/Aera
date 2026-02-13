use std::io::{self, Write};
use std::process::Command;
use std::env;

fn main() {
    loop {
        // Prompt
        print!("rsh> ");
        io::stdout().flush().unwrap();

        // Read input
        let mut line = String::new();
        if io::stdin().read_line(&mut line).is_err() {
            break;
        }
        let line = line.trim();
        if line.is_empty() {
            continue;
        }

        // Very simple tokenization (whitespace only)
        let mut parts = line.split_whitespace();
        let cmd = match parts.next() {
            Some(c) => c,
            None => continue,
        };
        let args: Vec<&str> = parts.collect();

        // Builtins
        match cmd {
            "exit" => break,
            "cd" => {
                let target = args.get(0).copied().unwrap_or("/");
                if let Err(e) = env::set_current_dir(target) {
                    eprintln!("cd: {e}");
                }
                continue;
            }
            _ => {}
        }

        // Run external command
        match Command::new(cmd).args(&args).status() {
            Ok(status) => {
                if !status.success() {
                    eprintln!("exit status: {status}");
                }
            }
            Err(e) => eprintln!("error: {e}"),
        }
    }
}