require "rubygems"
require "bundler"
Bundler.setup


require 'albacore'
require 'version_bumper'


TOOLS = [
          :qcount,
          #:qcp,
          :qgrep,
          :qls,
          :qrm,
          :qtail,
          :qtouch,
          :qtruncate
        ]
PROJECT = 'qtools'


desc "Build"
msbuild :build => :assemblyinfo do |msb|
  msb.properties :configuration => :Release
  msb.targets :Clean, :Build
  msb.solution = "#{PROJECT}.sln"
end


output :output => [:build, :merge_all] do |out|
  out.from '.'
  out.to 'out'

  TOOLS.each do |t|
    out.file "#{t}/bin/release/merged/#{t}.exe", :as=>"#{t}.exe"
  end

  out.file 'LICENSE.txt'
  out.file 'README.md'
  out.file 'VERSION'
  out.erb "build/#{PROJECT}.nuspec.erb", :as => "#{PROJECT}.nuspec", :locals => { :version => bumper_version }
end

desc "Create Zip for Github"
zip :zip => :output do | zip |
    zip.directories_to_zip "out"
    zip.output_file = "#{PROJECT}.v#{bumper_version.to_s}.zip"
    zip.output_path = File.dirname(__FILE__)
end

desc "Create nuget pacakge"
task :nu => :output do
  `tools/nuget/nuget.exe p out/#{PROJECT}.nuspec`	
end


TOOLS.each do |t|
  exec "merge_#{t}" do |cmd|
    cmd.command = 'tools\ilmerge\ilmerge.exe'
    mkdir "#{t}/bin/release/merged" if !File.exist?("#{t}/bin/release/merged")
    cmd.parameters ="/targetplatform:v4,C:/Windows/Microsoft.NET/Framework/v4.0.30319 /out:#{t}/bin/release/merged/#{t}.exe #{t}/bin/release/#{t}.exe #{t}/bin/release/#{PROJECT}.Core.dll #{t}/bin/release/CommandLine.dll"
    #cmd.parameters ="/out:#{t}/bin/release/merged/#{t}.exe #{t}/bin/release/#{t}.exe #{t}/bin/release/#{PROJECT}.Core.dll #{t}/bin/release/CommandLine.dll"
    puts "merged #{t}."
  end
end


task :merge_all => TOOLS.map{|t| "merge_#{t}" }


assemblyinfo :assemblyinfo do |asm|
  asm.version = bumper_version.to_s
  asm.file_version = bumper_version.to_s
  asm.company_name = "Paracode"
  asm.copyright = "Dotan Nahum, Paracode (c) 2010-2011"
  asm.output_file = "AssemblyInfo.cs"
end

