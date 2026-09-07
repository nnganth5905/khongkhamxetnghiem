import React from 'react';

export default function NewsVoucher() {
  return (
    <div
      className="promotion-page py-4"
      style={{
        fontFamily:
          'Arial, sans-serif',

        lineHeight:
          '1.6',

        background:
          '#f9f9f9',

        minHeight:
          '100vh',
      }}
    >
      <div
        className="container"
        style={{
          maxWidth:
            '800px',

          margin:
            '30px auto',

          background:
            '#fff',

          padding:
            '30px',

          borderRadius:
            '8px',

          boxShadow:
            '0 4px 10px rgba(0, 0, 0, 0.1)',
        }}
      >
        <h1
          className="fw-bold"
          style={{
            color:
              '#0066cc',

            marginBottom:
              '15px',
          }}
        >
          Tặng voucher 200k khi đặt lịch online
        </h1>

        <p
          style={{
            marginBottom:
              '15px',

            textAlign:
              'justify',
          }}
        >
          Khách hàng đặt lịch khám qua website sẽ
          được tặng ngay{' '}

          <span
            className="fw-bold"
            style={{
              color:
                '#d9534f',
            }}
          >
            voucher 200,000đ
          </span>{' '}

          cho lần sử dụng dịch vụ tiếp theo. Đây là
          ưu đãi đặc biệt nhằm khuyến khích khách
          hàng trải nghiệm dịch vụ đặt lịch trực
          tuyến nhanh chóng, tiện lợi và tiết kiệm
          thời gian chờ đợi.
        </p>

        <p
          style={{
            marginBottom:
              '15px',

            textAlign:
              'justify',
          }}
        >
          Ưu đãi áp dụng theo điều kiện của chương
          trình tại Bio Medic Center. Voucher có thể
          được áp dụng cho những dịch vụ đủ điều kiện
          theo quy định của chương trình.
        </p>

        <p
          style={{
            marginBottom:
              0,

            textAlign:
              'justify',
          }}
        >
          Đặt lịch ngay hôm nay để nhận quà tặng hấp
          dẫn này!
        </p>
      </div>
    </div>
  );
}