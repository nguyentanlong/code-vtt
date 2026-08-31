# Sử dụng môi trường Mono 6.12 gốc
FROM mono:6.12
#FROM docker.io/library/mono:6.12@sha256:34d816779b1248b5cfd095770b64ecbaf1798e2aca693a91c11a018dce9

# Bước 1: Sửa triệt để sources.list sang archive và tắt cơ chế kiểm tra bảo mật lỗi thời
#RUN echo "deb http://archive.debian.org/debian buster main" > /etc/apt/sources.list && \
#    echo "deb http://archive.debian.org/debian-security buster/updates main" >> /etc/apt/sources.list && \
#    echo "deb http://archive.debian.org/debian buster-updates main" >> /etc/apt/sources.list && \
#    echo 'Acquire::Check-Valid-Until "false";' > /etc/apt/apt.conf.d/99no-check-valid-until
RUN sed -i 's|deb.debian.org|archive.debian.org|g; s|security.debian.org|archive.debian.org|g' /etc/apt/sources.list && \
    sed -i '/buster-updates/d' /etc/apt/sources.list && \
    apt-get update -o Acquire::Check-Valid-Until=false --allow-releaseinfo-change && \
    apt-get install -y mono-xsp4 --allow-unauthenticated && \
    apt-get clean

# Bước 2: Cập nhật danh sách với cờ cho phép đổi thông tin bản phát hành và cài đặt xsp4
RUN apt-get update --allow-releaseinfo-change && \
    apt-get install -y mono-xsp4 --allow-unauthenticated && \
    apt-get clean

# Bước 3: Thiết lập thư mục làm việc và nạp toàn bộ code vào
WORKDIR /app
COPY . /app

# Mở cổng mạng kết nối 5959
EXPOSE 5959

# Kích hoạt máy chủ ảo XSP4 chạy chính xác trên cổng 5959
CMD ["xsp4", "--nonstop", "--port", "5959"]

