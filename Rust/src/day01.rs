// Theme: match location ids by similarity score

#[cfg(test)]
mod day01 {

    use std::collections::HashMap;
    use std::env::current_exe;
    use std::fs::read_to_string;
    use std::hash::Hash;
    use std::path::Path;

    use itertools::Itertools;

    fn read_aoc_input(filename: &str) -> Vec<String> {
        read_to_string(filename).expect("file not found")
                                .lines()
                                .map(|line| line.to_string())
                                .collect()
    }

    fn parse_data(lines: Vec<String>) -> (Vec<u32>, Vec<u32>) {
        lines.iter()
             .map(|line| {
                 line.split_ascii_whitespace()
                     .map(|n| n.parse::<u32>().unwrap())
                     .collect_tuple()
                     .unwrap()
              })
             .unzip()
    }

    #[test]
    fn day01_samples() {
        let data: Vec<String> = vec![
            "3   4".to_string(),
            "4   3".to_string(),
            "2   5".to_string(),
            "1   3".to_string(),
            "3   9".to_string(),
            "3   3".to_string(),
        ];
        let (left, right) = parse_data(data);

        let result_puzzle_1 = puzzle1(&left, &right);
        assert_eq!(result_puzzle_1, 2 + 1 + 0 + 1 + 2 + 5);

        let result_puzzle_2 = puzzle2(&left, &right);
        assert_eq!(result_puzzle_2, 9 + 4 + 0 + 0 + 9 + 9);
    }

    #[test]
    fn day01_aoc_input() {
        let cur_exe = current_exe().unwrap().into_os_string().into_string().unwrap();
        let cur_dir: String = Path::new(&cur_exe).parent().unwrap().as_os_str().to_str().unwrap().into();
        let data_file = cur_dir + "\\.input\\day01.data";

        let data: Vec<String> = read_aoc_input(&data_file);
        let (left, right) = parse_data(data);

        let result_puzzle1 = puzzle1(&left, &right);
        assert_eq!(result_puzzle1, 1941353);

        let result_puzzle_2 = puzzle2(&left, &right);
        assert_eq!(result_puzzle_2, 22539317);
    }

    // Throughout the Chief's office, the historically significant locations are listed not by name but by a unique number called the location ID. To make
    // sure they don't miss anything, The Historians split into two groups, each searching the office and trying to create their own complete list of location
    // IDs. There's just one problem: by holding the two lists up side by side (your puzzle input), it quickly becomes clear that the lists aren't very similar.
    // Maybe the lists are only off by a small amount! To find out, pair up the numbers and measure how far apart they are. Pair up the smallest number in the
    // left list with the smallest number in the right list, then the second-smallest left number with the second-smallest right number, and so on. Within each
    // pair, figure out how far apart the two numbers are; you'll need to add up all of those distances. 
    //
    // Puzzle == Your actual left and right lists contain many location IDs. What is the total distance between your lists?
    fn puzzle1(left: &Vec<u32>, right: &Vec<u32>) -> u32 {
        let mut left_sorted = left.to_vec();
        left_sorted.sort();
        let mut right_sorted = right.to_vec();
        right_sorted.sort();

        left_sorted.iter()
                   .zip(right_sorted.iter())
                   .map(|(x, y)| delta(x, y))
                   .sum()
    }

    fn delta(left: &u32, right: &u32) -> u32 {
        if left > right { left - right } else { right - left}
    }

    // This time, you'll need to figure out exactly how often each number from the left list appears in the right list. Calculate a total similarity score by
    // adding up each number in the left list after multiplying it by the number of times that number appears in the right list.
    //
    // Puzzle == Once again consider your left and right lists. What is their similarity score?
    fn puzzle2(left: &Vec<u32>, right: &Vec<u32>) -> u32 {
        let right_grouped = group_by(right.to_vec(), |i| i * 1);

        left.iter()
            .map(|l| match right_grouped.get(&l) {
                Some(x) => x.len() as u32 * l,
                None => 0,
             })
            .sum()
    }

    fn group_by<T, K, F>(x: T, f: F) -> HashMap<K, Vec<T::Item>>
    where T: IntoIterator,
          F: Fn(&T::Item) -> K,
          K: Eq + Hash
    {
        let mut map = HashMap::new();
        for item in x {
            let hash = f(&item);
            map.entry(hash).or_insert(vec![]).push(item);
        }
        map
    }
}
